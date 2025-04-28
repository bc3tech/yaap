namespace Yaap.Server.Abstractions;

using System.Text.Json;
using System.Threading;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Yaap.Core.Models;
using Yaap.Server;

/// <summary>
/// Represents an abstract base class for a Yaap server that handles client Hellos and Goodbyes and notifications.
/// Implements the <see cref="IHostedService"/> interface for server lifecycle management.
/// </summary>
public abstract class BaseYaapServer : IHostedService, IYaapServer
{
    private readonly ILogger? _log;

    /// <summary>
    /// Gets the logger instance used for logging messages and events.
    /// </summary>
    protected ILogger? Log { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseYaapServer"/> class with the specified distributed cache and
    /// optional logger factory.
    /// </summary>
    /// <param name="distributedCache">The distributed cache used for storing client-related data.</param>
    /// <param name="loggerFactory">An optional factory for creating loggers. If null, logging will be disabled.</param>
    public BaseYaapServer(IDistributedCache distributedCache, ILoggerFactory? loggerFactory)
    {
        _log = loggerFactory?.CreateLogger("Yaap.Server");
        this.Log = loggerFactory?.CreateLogger($"Yaap.Server.{GetType().Name}");

        this.ClientCache = distributedCache;
    }

    /// <summary>
    /// Gets the collection of connected Yaap clients.
    /// </summary>
    protected IDistributedCache ClientCache { get; }

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task HandleHelloAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
    {
        if (this.ClientCache.GetString(clientDetail.Name) is not null)
        {
            // Client already exists, handle accordingly
            throw new ArgumentException("Client already exists", nameof(clientDetail));
        }

        _log?.AddingClientYaapClientNameToCache(clientDetail.Name);
        await this.ClientCache.SetStringAsync(clientDetail.Name, JsonSerializer.Serialize(clientDetail), cancellationToken).ConfigureAwait(false);
        _log?.ClientAddedToCacheYaapClientDetail(clientDetail);

        await HandleHelloCustomAsync(clientDetail, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles a "Hello" instruction from a Yaap client after the client has been added to the list of clients.
    /// This method is invoked when a client sends a greeting or initialization message.
    /// </summary>
    /// <param name="clientDetail">Details of the Yaap client sending the instruction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task HandleHelloCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task HandleGoodbyeAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
    {
        _log?.RemovingClientYaapClientNameFromCache(clientDetail.Name);
        await this.ClientCache.RemoveAsync(clientDetail.Name, cancellationToken).ConfigureAwait(false);
        _log?.ClientYaapClientNameRemovedFromCache(clientDetail.Name);

        await HandleGoodbyeCustomAsync(clientDetail, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles a "Goodbye" notification from a Yaap client after the client has been removed from the internal cache.
    /// This method is invoked when a client sends a shutdown or exit notification.
    /// </summary>
    /// <param name="clientDetail">Details of the Yaap client sending the notification.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task HandleGoodbyeCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken) => Task.CompletedTask;
}

/// <summary>
/// Represents an abstract base class for a Yaap server that handles client Hellos and Goodbyes and notifications.
/// Implements the <see cref="IHostedService"/> interface for server lifecycle management.
/// </summary>
public abstract class BaseYaapServer<THelloResponse> : IHostedService, IYaapServer<YaapClientDetail, THelloResponse> where THelloResponse : notnull
{
    private readonly ILogger? _log;

    /// <summary>
    /// Gets the logger instance used for logging messages and events.
    /// </summary>
    protected ILogger? Log { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseYaapServer"/> class with the specified distributed cache and
    /// optional logger factory.
    /// </summary>
    /// <param name="distributedCache">The distributed cache used for storing client-related data.</param>
    /// <param name="loggerFactory">An optional factory for creating loggers. If null, logging will be disabled.</param>
    public BaseYaapServer(IDistributedCache distributedCache, ILoggerFactory? loggerFactory)
    {
        _log = loggerFactory?.CreateLogger("Yaap.Server");
        this.Log = loggerFactory?.CreateLogger($"Yaap.Server.{GetType().Name}");

        this.ClientCache = distributedCache;
    }

    /// <summary>
    /// Gets the collection of connected Yaap clients.
    /// </summary>
    protected IDistributedCache ClientCache { get; }

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task<THelloResponse> HandleHelloAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
    {
        if (this.ClientCache.GetString(clientDetail.Name) is not null)
        {
            // Client already exists, handle accordingly
            throw new ArgumentException("Client already exists", nameof(clientDetail));
        }

        _log?.AddingClientYaapClientNameToCache(clientDetail.Name);
        await this.ClientCache.SetStringAsync(clientDetail.Name, JsonSerializer.Serialize(clientDetail), cancellationToken).ConfigureAwait(false);
        _log?.ClientAddedToCacheYaapClientDetail(clientDetail);

        return await HandleHelloCustomAsync(clientDetail, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles a "Hello" instruction from a Yaap client after the client has been added to the list of clients.
    /// This method is invoked when a client sends a greeting or initialization message.
    /// </summary>
    /// <param name="clientDetail">Details of the Yaap client sending the instruction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task<THelloResponse> HandleHelloCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken);

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task HandleGoodbyeAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
    {
        _log?.RemovingClientYaapClientNameFromCache(clientDetail.Name);
        await this.ClientCache.RemoveAsync(clientDetail.Name, cancellationToken).ConfigureAwait(false);
        _log?.ClientYaapClientNameRemovedFromCache(clientDetail.Name);

        await HandleGoodbyeCustomAsync(clientDetail, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles a "Goodbye" notification from a Yaap client after the client has been removed from the internal cache.
    /// This method is invoked when a client sends a shutdown or exit notification.
    /// </summary>
    /// <param name="clientDetail">Details of the Yaap client sending the notification.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task HandleGoodbyeCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken) => Task.CompletedTask;
}
