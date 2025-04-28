namespace Yaap.Client.Abstractions;

using System;
using System.Threading;

using A2A.Models;

using Task = Task;

/// <summary>
/// Defines the contract for a YAAP (Yet Another Awesome Protocol) client.
/// </summary>
public interface IYaapClient : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Gets the details of the YAAP client.
    /// </summary>
    AgentCard Detail { get; }

    /// <summary>
    /// Gets the endpoint URI of the YAAP server.
    /// </summary>
    Uri YaapServerEndpoint { get; }

    /// <summary>
    /// Sends a goodbye message asynchronously to the YAAP server.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SayGoodbyeAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Sends a hello message asynchronously to the YAAP server.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SayHelloAsync(CancellationToken cancellationToken);
}
