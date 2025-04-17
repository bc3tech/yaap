namespace Yaap.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Threading;
using System.Threading.Tasks;

using Yaap.Common;
using Yaap.Models;

/// <summary>
/// Represents an abstract base class for a Yaap client that implements the <see cref="IHostedLifecycleService"/> interface.
/// Provides lifecycle methods and configuration for interacting with a Yaap server.
/// </summary>
public abstract class YaapClient(IServiceProvider services) : IHostedLifecycleService
{
    /// <summary>
    /// Gets the service provider used to resolve dependencies.
    /// </summary>
    protected IServiceProvider Services { get; } = services;

    /// <summary>
    /// Gets the details of the Yaap client, including its name, description, and callback URL.
    /// </summary>
    protected YaapClientDetail Detail { get; } = new(
        Throws.IfNullOrWhiteSpace(services.GetRequiredService<IConfiguration>()["Yaap:Client:Name"]),
        Throws.IfNullOrWhiteSpace(services.GetRequiredService<IConfiguration>()["Yaap:Client:Description"]),
        services.GetRequiredService<IConfiguration>()["Yaap:Client:CallbackUrl"] is string callbackUrl
            ? new Uri(callbackUrl)
            : null
        );

    /// <summary>
    /// Gets the endpoint URI of the Yaap server.
    /// </summary>
    protected Uri YaapServerEndpoint { get; } = new(
        Throws.IfNullOrWhiteSpace(services.GetRequiredService<IConfiguration>()["Yaap:Server:Endpoint"]));

    /// <inheritdoc/>
    public virtual Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task StartedAsync(CancellationToken cancellationToken) => SayHelloAsync(cancellationToken);

    /// <summary>
    /// Sends the introduction message to the Yaap server.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task SayHelloAsync(CancellationToken cancellationToken);

    /// <inheritdoc/>
    public virtual Task StoppingAsync(CancellationToken cancellationToken) => SayGoodbyeAsync(cancellationToken);

    /// <summary>
    /// Sends the shutdown/exit message to the Yaap server for this client.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task SayGoodbyeAsync(CancellationToken cancellationToken);

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
