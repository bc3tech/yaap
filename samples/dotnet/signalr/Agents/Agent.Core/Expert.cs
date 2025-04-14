namespace Common;

using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Agent.Core;

using Common.Extensions;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Models.SignalR;

public abstract class Expert : IHostedService
{
    protected HubConnection SignalR { get; private set; } = default!;

    protected readonly IConfiguration _config;
    protected readonly ILogger _log;
    protected readonly Kernel _kernel;
    protected readonly PromptExecutionSettings _promptSettings;
    protected readonly IHttpClientFactory _httpFactory;

    protected Expert(
        [NotNull] IConfiguration appConfig,
        [NotNull] ILoggerFactory loggerFactory,
        [NotNull] IHttpClientFactory httpClientFactory,
        [NotNull] Kernel sk,
        [NotNull] PromptExecutionSettings promptSettings)
    {
        _config = Throws.IfNull(appConfig);
        _kernel = Throws.IfNull(sk);
        _promptSettings = Throws.IfNull(promptSettings);
        _httpFactory = Throws.IfNull(httpClientFactory);

        this.Name = Throws.IfNullOrWhiteSpace(appConfig[Constants.Configuration.Paths.AgentName]);
        this.Description = appConfig[Constants.Configuration.Paths.AgentDescription];

        _log = Throws.IfNull(loggerFactory).CreateLogger(this.Name);
    }

    public string Name { get; protected init; }
    public string? Description { get; protected init; }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        AgentDefinition.OutputRegisteredSkFunctions(_kernel, new LogTraceTextWriter(_log));

        await ConnectToSignalRAsync(cancellationToken);

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        _kernel.FunctionInvocationFilters.Add(new DefaultFunctionInvocationFilter(_log, this.SignalR));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }

    protected virtual async Task AfterSignalRConnectedAsync()
    {
        if (!bool.Parse(await this.SignalR.InvokeAsync<string>("Connect")))
        {
            _log.LogError("Connection failed");
        }

        if (this.PerformsIntroduction)
        {
            await IntroduceAsync();
            this.SignalR.On(Constants.SignalR.Functions.Reintroduce, IntroduceAsync);
        }

        this.SignalR.On<string, string>(Constants.SignalR.Functions.GetAnswer, GetAnswerAsync);

        _log.LogInformation("Awaiting question...");
    }

    protected virtual bool PerformsIntroduction { get; } = true;

    protected async Task IntroduceAsync()
    {
        _log.LogDebug("Introducing myself...");
        await this.SignalR.SendAsync(Constants.SignalR.Functions.Introduce, this.Name, _config[Constants.Configuration.Paths.AgentDescription]);
    }

    private async Task ConnectToSignalRAsync(CancellationToken cancellationToken)
    {
        using IDisposable scope = _log.CreateMethodScope();
        _log.LogDebug("Connecting to SignalR hub...");

        ConnectionInfo? connInfo = default;
        using (IDisposable? negotiationScope = _log.BeginScope("negotiation"))
        {
            var targetEndpoint = $@"{Throws.IfNullOrWhiteSpace(_config[Constants.Configuration.VariableNames.SignalREndpoint])}?userid={this.Name}";
            HttpClient client = _httpFactory.CreateClient("negotiation");
            HttpResponseMessage hubNegotiateResponse = new();
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    hubNegotiateResponse = await client.PostAsync(targetEndpoint, null, cancellationToken);
                    break;
                }
                catch (Exception e)
                {
                    _log.LogDebug(e, $@"Negotiation failed");
                    await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                }
            }

            if (hubNegotiateResponse is null)
            {
                _log.LogCritical("Unable to connect to server {signalrHubEndpoint} - Exiting.", _config[Constants.Configuration.VariableNames.SignalREndpoint]);
                return;
            }

            hubNegotiateResponse.EnsureSuccessStatusCode();

            try
            {
                connInfo = await hubNegotiateResponse.Content.ReadFromJsonAsync<ConnectionInfo>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex, "Error parsing negotiation response");
                _log.LogCritical("Unable to connect to server {signalrHubEndpoint} - Exiting.", _config[Constants.Configuration.VariableNames.SignalREndpoint]);
                return;
            }
        }

        ArgumentNullException.ThrowIfNull(connInfo);

        IHubConnectionBuilder builder = new HubConnectionBuilder()
            .WithUrl(connInfo.Url, o => o.AccessTokenProvider = connInfo.GetAccessToken)
            .ConfigureLogging(lb =>
            {
                lb.AddConfiguration(_config.GetSection("Logging"));
                lb.AddSimpleConsole(o =>
                {
                    o.SingleLine = true;
                    o.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                    o.IncludeScopes = true;
                });
            }).WithAutomaticReconnect()
#if DEBUG
            .WithServerTimeout(TimeSpan.FromMinutes(5))
#endif
            .WithStatefulReconnect();

        this.SignalR = builder.Build();
        await this.SignalR.StartAsync(cancellationToken);

        await AfterSignalRConnectedAsync();
    }

    protected Task<string> GetAnswerAsync(string prompt)
    {
        using IDisposable scope = _log.CreateMethodScope();

        return GetAnswerInternalAsync(prompt);
    }

    protected virtual Task<string> GetAnswerInternalAsync(string prompt)
    {
        return ExecuteWithThrottleHandlingAsync(async () =>
        {
            string response;
            try
            {
                FunctionResult promptResult = await _kernel.InvokePromptAsync(prompt, new(_promptSettings));

                _log.LogDebug("Prompt handled. Response: {promptResponse}", promptResult);

                response = promptResult.ToString();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error handling prompt: {prompt}", prompt);

                response = JsonSerializer.Serialize(ex.Message);
            }

            return response;
        });
    }

    protected async Task<T> ExecuteWithThrottleHandlingAsync<T>(Func<Task<T>> operation, int maxRetries = 10)
    {
        Exception? lastException = null;
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                return await operation();
            }
            catch (HttpOperationException ex)
            {
                lastException = ex;
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests && ex.InnerException is Azure.RequestFailedException rex)
                {
                    Azure.Response? resp = rex.GetRawResponse();
                    if (resp?.Headers.TryGetValue("Retry-After", out var waitTime) is true)
                    {
                        _log.LogWarning("Responses Throttled! Waiting {retryAfter} seconds to try again...", waitTime);
                        await Task.Delay(TimeSpan.FromSeconds(int.Parse(waitTime))).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        _log.LogError(lastException!, "Max retries exceeded.");
        throw lastException!;
    }

    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
