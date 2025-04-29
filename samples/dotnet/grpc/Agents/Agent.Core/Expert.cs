namespace Agent.Core;

using System.Threading;
using System.Threading.Tasks;

using Common;
using Common.Extensions;

using Grpc.Core;
using Grpc.Models;
using Grpc.Orchestrator;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Yaap.Client.Abstractions;
using Yaap.Core.Models;

using YaapClientDetail = Yaap.Core.Models.YaapClientDetail;

public abstract class Expert : Grpc.Expert.Expert.ExpertBase, IYaapClient, IHostedService
{
    protected readonly IConfiguration _config;
    protected readonly ILogger _log;
    protected readonly Kernel _kernel;
    protected readonly PromptExecutionSettings _promptSettings;
    protected readonly IHttpClientFactory _httpFactory;
    private bool disposedValue;

    protected Orchestrator.OrchestratorClient? Client { get; }

    protected Expert(IConfiguration appConfig, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, Kernel sk, PromptExecutionSettings promptSettings, Orchestrator.OrchestratorClient? orchestrator = null)
    {
        _config = Throws.IfNull(appConfig);
        _kernel = Throws.IfNull(sk);
        _promptSettings = Throws.IfNull(promptSettings);
        _httpFactory = Throws.IfNull(httpClientFactory);

        this.Detail = new
        (
            Throws.IfNullOrWhiteSpace(appConfig[Constants.Configuration.Paths.AgentName]),
            appConfig[Constants.Configuration.Paths.AgentDescription] ?? string.Empty,
            orchestrator is null ? null : new(Throws.IfNullOrWhiteSpace(appConfig["URLS"])!.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)![0])
        );

        _log = loggerFactory.CreateLogger(this.Detail.Name);
        this.Client = orchestrator;
    }

    public YaapClientDetail Detail { get; protected init; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        AgentDefinition.OutputRegisteredSkFunctions(_kernel, new LogTraceTextWriter(_log));
        if (this.Client is not null)
        {
            _kernel.FunctionInvocationFilters.Add(new DefaultFunctionInvocationFilter(_log, this.Client));

            _log.IntroducingMyself();
            await this.Client.HelloAsync(this.Detail, cancellationToken: cancellationToken);
        }

        _log.AwaitingQuestion();
    }

    public override async Task<AnswerResponse> GetAnswer(AnswerRequest request, ServerCallContext context)
    {
        using IDisposable scope = _log.CreateMethodScope();

        var answer = await AIHelpers.GetAnswerAsync(_kernel, _promptSettings, request.Prompt, context.CancellationToken, _log).ConfigureAwait(false);
        return new() { Completion = answer };
    }

    protected virtual bool PerformsIntroduction { get; } = true;

    public Uri YaapServerEndpoint => throw new NotImplementedException();

    public override Task GetAnswerStream(AnswerRequest request, IServerStreamWriter<StreamResponse> responseStream, ServerCallContext context) => AIHelpers.GetAnswerStreamingAsync(_kernel, _promptSettings, request.Prompt, responseStream, context.CancellationToken, _log);

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        if (this.Client is not null)
        {
            _log.SayingGoodbyeToOrchestrator();
            await SayGoodbyeAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public virtual async Task SayHelloAsync(CancellationToken cancellationToken) => await this.Client!.HelloAsync(this.Detail, cancellationToken: cancellationToken);

    public virtual async Task SayGoodbyeAsync(CancellationToken cancellationToken) => await this.Client!.GoodbyeAsync(this.Detail, cancellationToken: cancellationToken);

    public async ValueTask DisposeAsync()
    {
        await SayGoodbyeAsync(default);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Expert()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
