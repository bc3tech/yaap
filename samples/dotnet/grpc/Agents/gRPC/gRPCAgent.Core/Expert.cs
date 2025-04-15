namespace gRPCAgent.Core;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Common;
using Common.Extensions;

using Expert_gRPC;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;
using Grpc.Net.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Orchestrator_gRPC;

public abstract class Expert : Agent_gRPC.Agent.AgentBase, IHostedService
{
    protected readonly IConfiguration _config;
    protected readonly ILogger _log;
    protected readonly Kernel _kernel;
    protected readonly PromptExecutionSettings _promptSettings;
    protected readonly IHttpClientFactory _httpFactory;
    protected Orchestrator.OrchestratorClient Client { get; }

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

        this.Detail = new()
        {
            Name = Throws.IfNullOrWhiteSpace(appConfig[Constants.Configuration.Paths.AgentName]),
            Description = appConfig[Constants.Configuration.Paths.AgentDescription] ?? string.Empty,
            CallbackAddress = appConfig["URLS"]!.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)![0]
        };

        _log = Throws.IfNull(loggerFactory).CreateLogger(this.Detail.Name);

        if (string.IsNullOrWhiteSpace(_config[Constants.Configuration.VariableNames.SignalREndpoint]))
        {
            _log.LogInformation("No SignalR endpoint configured. This agent will not be able to receive questions from the orchestrator.");
        }
        else
        {
            this.Client = new Orchestrator.OrchestratorClient(GrpcChannel.ForAddress(Throws.IfNullOrWhiteSpace(_config[Constants.Configuration.VariableNames.SignalREndpoint])));
        }
    }

    public AgentDetail Detail { get; protected init; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        AgentDefinition.OutputRegisteredSkFunctions(_kernel, new LogTraceTextWriter(_log));

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        if (this.Client is not null)
        {
            _kernel.FunctionInvocationFilters.Add(new DefaultFunctionInvocationFilter(_log, this.Client));
        }
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        if (this.PerformsIntroduction)
        {
            await IntroduceAsync(cancellationToken);
        }

        _log.AwaitingQuestion();
    }

    public override async Task<Empty> Reintroduce(Empty request, ServerCallContext context)
    {
        await IntroduceAsync(context.CancellationToken);
        return new Empty();
    }
    public async Task<Expert_gRPC.AnswerResponse> GetAnswer(string prompt, CancellationToken cancellationToken)
    {
        using IDisposable scope = _log.CreateMethodScope();

        var answer = await GetAnswerInternalAsync(prompt, cancellationToken);
        return new() { Completion = answer };
    }

    public override Task<Expert_gRPC.AnswerResponse> GetAnswer(Expert_gRPC.AnswerRequest request, ServerCallContext context) => GetAnswer(request.Prompt, context.CancellationToken);

    protected virtual bool PerformsIntroduction { get; } = true;

    protected async Task IntroduceAsync(CancellationToken cancellationToken)
    {
        _log.IntroducingMyself();
        await this.Client.IntroduceAsync(this.Detail, new(cancellationToken: cancellationToken));
    }

    protected virtual Task<string> GetAnswerInternalAsync(string prompt, CancellationToken cancellationToken)
    {
        return ExecuteWithThrottleHandlingAsync(async () =>
        {
            string response;
            try
            {
                FunctionResult promptResult = await _kernel.InvokePromptAsync(prompt, new(_promptSettings));

                _log.PromptHandledResponsePromptResponse(promptResult);

                response = promptResult.ToString();
            }
            catch (Exception ex)
            {
                _log.ErrorHandlingPromptPrompt(ex, prompt);

                response = JsonSerializer.Serialize(ex.Message);
            }

            return response;
        }, cancellationToken);
    }

    public override Task GetAnswerStream(AnswerRequest request, IServerStreamWriter<StreamResponse> responseStream, ServerCallContext context)
    {
        var prompt = request.Prompt;
        return ExecuteWithThrottleHandlingAsync(async () =>
        {
            string response;
            try
            {
                await foreach (StreamingKernelContent token in _kernel.InvokePromptStreamingAsync(prompt, new(_promptSettings)))
                {
                    await responseStream.WriteAsync(new() { Token = token.ToString() });
                }
            }
            catch (Exception ex)
            {
                _log.ErrorHandlingPromptPrompt(ex, prompt);

                response = JsonSerializer.Serialize(ex.Message);
            }
        }, context.CancellationToken);
    }

    protected async Task<T> ExecuteWithThrottleHandlingAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken, int maxRetries = 10)
    {
        Exception? lastException = null;
        for (var i = 0; i < maxRetries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
                await operation();
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

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        _log.SayingGoodbyeToOrchestrator();
        await this.Client.GoodbyeAsync(this.Detail, new CallOptions(cancellationToken: cancellationToken));
    }
}
