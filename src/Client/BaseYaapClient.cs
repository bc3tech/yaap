namespace Yaap.Client.Abstractions;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Yaap.Client;
using Yaap.Common;
using Yaap.Core.Models;

using Task = Task;

/// <summary>
/// Represents an abstract base class for a Yaap client that implements the <see cref="IHostedLifecycleService"/> interface.
/// Provides lifecycle methods and configuration for interacting with a Yaap server.
/// </summary>
public abstract class BaseYaapClient : IHostedLifecycleService, IYaapClient
{
    private readonly ILogger? _log;
    private readonly IConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseYaapClient"/> class.
    /// </summary>
    /// <param name="appConfig">The application configuration used to initialize the client.</param>
    /// <param name="clientDetail"></param>
    /// <param name="loggerFactory">The logger factory used to create loggers for the client. Optional.</param>
    /// <exception cref="ArgumentNullException">Thrown if required configuration values are null or empty.</exception>
    protected BaseYaapClient(IConfiguration appConfig, YaapClientDetail? clientDetail, ILoggerFactory? loggerFactory)
    {
        _config = appConfig;
        Throws.IfNullOrWhiteSpace(clientDetail?.Name, "Yaap client name is required.");
        Throws.IfNullOrWhiteSpace(clientDetail.Description, "Yaap client description is required.");

        this.Detail = clientDetail!;

        this.YaapServerEndpoint = new(Throws.IfNullOrWhiteSpace(_config["Yaap:Server:Endpoint"]));
        _log = loggerFactory?.CreateLogger($"Yaap.Client.{this.Detail.Name}");
    }

    /// <summary>
    /// Gets the details of the Yaap client, including its name, description, and callback URL.
    /// </summary>
    public YaapClientDetail Detail { get; }

    /// <summary>
    /// Gets the endpoint URI of the Yaap server.
    /// </summary>
    public Uri YaapServerEndpoint { get; }

    /// <inheritdoc/>
    public virtual Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        await StartedInternalAsync(cancellationToken);

        _log?.IntroducingMyselfToTheYAAPServer();
        await SayHelloAsync(cancellationToken);
        _log?.IntroductionSuccessfulToYAAPServerAtYaapServerEndpoint(this.YaapServerEndpoint);
    }

    /// <summary>
    /// Triggered after <see cref="IHostedService.StartAsync(CancellationToken)"/> and before <see cref="SayHelloAsync(CancellationToken)"/>.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected virtual Task StartedInternalAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// Sends the introduction message to the Yaap server.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public abstract Task SayHelloAsync(CancellationToken cancellationToken);

    /// <inheritdoc/>
    public async Task StoppingAsync(CancellationToken cancellationToken)
    {
        if (!_saidGoodbye)
        {
            _log?.SayingGoodbyeToYAAPServer();
            await SayGoodbyeAsync(cancellationToken);
            _saidGoodbye = true;
            _log?.GoodbyeMessageSentToYAAPServer();
        }

        await StoppingInternalAsync(cancellationToken);
    }

    /// <summary>
    /// Triggered before <see cref="IHostedService.StopAsync(CancellationToken)"/> and after <see cref="SayGoodbyeAsync(CancellationToken)"/> is called.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the stop process has been aborted.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected virtual Task StoppingInternalAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// Sends the shutdown/exit message to the Yaap server for this client.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public abstract Task SayGoodbyeAsync(CancellationToken cancellationToken);

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private bool _disposed, _saidGoodbye;
    private bool disposedValue;

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed || _saidGoodbye)
        {
            return;
        }

        _log?.DisposingOfYAAPClient();
        await SayGoodbyeAsync(CancellationToken.None);
        _log?.SaidGoodbyeToYAAPServerAsPartOfDispose();

        GC.SuppressFinalize(this);
        _disposed = true;
    }

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "<Pending>")]
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                DisposeAsync().GetAwaiter().GetResult();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~BaseYaapClient()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
