namespace wsAgent.Core;

using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Common;
using Common.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

public abstract class Expert : IHostedService
{
    protected readonly IConfiguration _config;
    protected readonly ILogger _log;
    protected readonly Kernel _kernel;
    protected readonly PromptExecutionSettings _promptSettings;
    protected readonly IHttpClientFactory _httpFactory;

    private readonly ClientWebSocket? _webSocket;

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
        var securePort = _config["ASPNETCORE_HTTPS_PORTS"];
        if (securePort is not null)
        {
            this.CallbackPort = new Uri(securePort).Port;
            this.Secured = true;
        }
        else
        {
            this.CallbackPort = int.TryParse(_config["ASPNETCORE_HTTP_PORTS"], out var p) ? p : 80;
        }

        _log = Throws.IfNull(loggerFactory).CreateLogger(this.Name);

        if (this.PerformsIntroduction)
        {
            _webSocket = new ClientWebSocket();
        }
    }

    public string Name { get; protected init; }
    public string? Description { get; protected init; }
    public int CallbackPort { get; protected init; }
    public bool Secured { get; protected init; }
    public string Hostname { get; protected init; } = Environment.MachineName;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        AgentDefinition.OutputRegisteredSkFunctions(_kernel, new LogTraceTextWriter(_log));

        if (this.PerformsIntroduction && _webSocket is not null)
        {
            var uri = new Uri(_config[Constants.Configuration.VariableNames.OrchestratorEndpoint]);
            await _webSocket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
            await IntroduceAsync(cancellationToken).ConfigureAwait(false);
        }

        _log.AwaitingQuestion();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken).ConfigureAwait(false);
        }

        _log.SayingGoodbyeToOrchestrator();
    }

    protected virtual bool PerformsIntroduction { get; } = true;

    protected async Task IntroduceAsync(CancellationToken cancellationToken)
    {
        _log.IntroducingMyself();
        var message = JsonSerializer.Serialize(new
        {
            action = "Introduce",
            detail = new { this.Name, this.Description, this.Hostname, this.CallbackPort, this.Secured }
        });
        await SendMessageAsync(message, cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> GetAnswerAsync(string prompt, CancellationToken cancellationToken)
    {
        using IDisposable scope = _log.CreateMethodScope();

        return await GetAnswerInternalAsync(prompt, cancellationToken).ConfigureAwait(false);
    }

    protected async Task<string> GetAnswerInternalAsync(string prompt, CancellationToken cancellationToken)
    {
        return await ExecuteWithThrottleHandlingAsync(async () =>
        {
            string response;
            try
            {
                FunctionResult promptResult = await _kernel.InvokePromptAsync(prompt, new(_promptSettings)).ConfigureAwait(false);

                _log.PromptHandledResponsePromptResponse(promptResult);

                response = promptResult.ToString();
            }
            catch (Exception ex)
            {
                _log.ErrorHandlingPromptPrompt(ex, prompt);

                response = JsonSerializer.Serialize(ex.Message);
            }

            return response;
        }, cancellationToken).ConfigureAwait(false);
    }

    private Task StreamAnswerAsync(WebSocket caller, string prompt, CancellationToken cancellationToken)
    {
        return ExecuteWithThrottleHandlingAsync(async () =>
        {
            string response;
            try
            {
                await foreach (StreamingKernelContent token in _kernel.InvokePromptStreamingAsync(prompt, new(_promptSettings)))
                {
                    await caller.SendAsync(token.ToByteArray(), WebSocketMessageType.Text, false, cancellationToken).ConfigureAwait(false);
                }

                await caller.SendAsync(Encoding.UTF8.GetBytes(string.Empty), WebSocketMessageType.Text, true, cancellationToken);
            }
            catch (Exception ex)
            {
                _log.ErrorHandlingPromptPrompt(ex, prompt);

                response = JsonSerializer.Serialize(ex.Message);
            }
        }, cancellationToken);
    }

    protected virtual async Task<string> ProcessMessageAsync(WebSocket caller, string message, CancellationToken cancellationToken)
    {
        JsonElement jsonObject = JsonDocument.Parse(message).RootElement;
        var action = jsonObject.GetProperty("action").GetString();

        switch (action)
        {
            case "GetAnswer":
                var prompt = Throws.IfNullOrWhiteSpace(jsonObject.GetProperty("prompt").GetString());
                var completion = await GetAnswerAsync(prompt, cancellationToken).ConfigureAwait(false);
                return JsonSerializer.Serialize(new { completion });

            // Add more cases for other actions
            case "StreamAnswer":
                prompt = Throws.IfNullOrWhiteSpace(jsonObject.GetProperty("prompt").GetString());
                await StreamAnswerAsync(caller, prompt, cancellationToken).ConfigureAwait(false);
                return string.Empty;

            default:
                return JsonSerializer.Serialize(new { error = "Unknown action" });
        }
    }

    private async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);
    }

    protected async Task<T> ExecuteWithThrottleHandlingAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken, int maxRetries = 10)
    {
        Exception? lastException = null;
        for (var i = 0; i < maxRetries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await operation().ConfigureAwait(false);
            }
            catch (HttpOperationException ex)
            {
                lastException = ex;
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests && ex.InnerException is Azure.RequestFailedException rex)
                {
                    Azure.Response? resp = rex.GetRawResponse();
                    if (resp?.Headers.TryGetValue("Retry-After", out var waitTime) is true)
                    {
                        _log.ResponsesThrottledWaitingRetryAfterSecondsToTryAgain(waitTime);
                        await Task.Delay(TimeSpan.FromSeconds(int.Parse(waitTime)), cancellationToken).ConfigureAwait(false);
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

        _log.MaxRetriesExceeded(lastException);
        throw lastException!;
    }

    protected async Task ExecuteWithThrottleHandlingAsync(Func<Task> operation, CancellationToken cancellationToken, int maxRetries = 10)
    {
        Exception? lastException = null;
        for (var i = 0; i < maxRetries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await operation().ConfigureAwait(false);
                return;
            }
            catch (HttpOperationException ex)
            {
                lastException = ex;
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests && ex.InnerException is Azure.RequestFailedException rex)
                {
                    Azure.Response? resp = rex.GetRawResponse();
                    if (resp?.Headers.TryGetValue("Retry-After", out var waitTime) is true)
                    {
                        _log.ResponsesThrottledWaitingRetryAfterSecondsToTryAgain(waitTime);
                        await Task.Delay(TimeSpan.FromSeconds(int.Parse(waitTime)), cancellationToken).ConfigureAwait(false);
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

        _log.MaxRetriesExceeded(lastException);
        throw lastException!;
    }

    public async Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);

        while (!result.CloseStatus.HasValue)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var response = await ProcessMessageAsync(webSocket, message, cancellationToken).ConfigureAwait(false);

            var responseBytes = Encoding.UTF8.GetBytes(response);
            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), result.MessageType, result.EndOfMessage, CancellationToken.None).ConfigureAwait(false);

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
        }

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None).ConfigureAwait(false);
    }
}
