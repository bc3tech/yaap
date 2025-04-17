namespace Yaap.Server;

using Microsoft.Extensions.Hosting;

using Models;

using System.Threading;

/// <summary>
/// Represents an abstract base class for a Yaap server that handles client Hellos and Goodbyes and notifications.
/// Implements the <see cref="IHostedService"/> interface for server lifecycle management.
/// </summary>
public abstract class YaapServer(IServiceProvider services) : IHostedService
{
    /// <summary>
    /// Gets the service provider used to resolve dependencies.
    /// </summary>
    protected IServiceProvider Services { get; } = services;

    /// <summary>
    /// Handles a "Hello" instruction from a Yaap client.
    /// This method is invoked when a client sends a greeting or initialization message.
    /// </summary>
    /// <param name="clientDetail">Details of the Yaap client sending the instruction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task HandleHelloAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken);

    /// <summary>
    /// Handles a "Goodbye" notification from a Yaap client.
    /// This method is invoked when a client sends a shutdown or exit notification.
    /// </summary>
    /// <param name="clientDetail">Details of the Yaap client sending the notification.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task HandleGoodbyeAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken);

    /// <inheritdoc/>
    public abstract Task StartAsync(CancellationToken cancellationToken);

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
