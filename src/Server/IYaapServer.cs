namespace Yaap.Server.Abstractions;

using System.Threading;
using System.Threading.Tasks;

using Yaap.Core.Models;

/// <summary>
/// Defines the contract for a Yaap server to handle client interactions and configure event handlers.
/// </summary>
public interface IYaapServer<TClientDetail>
{
    /// <summary>
    /// Handles a "Goodbye" notification from a Yaap client.
    /// This method is invoked when a client sends a shutdown or exit notification.
    /// </summary>
    /// <param name="clientDetail">Details of the Yaap client sending the notification.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleGoodbyeAsync(TClientDetail clientDetail, CancellationToken cancellationToken);

    /// <summary>
    /// Handles a "Hello" instruction from a Yaap client.
    /// This method is invoked when a client sends a greeting or initialization message.
    /// </summary>
    /// <param name="clientDetail">Details of the Yaap client sending the instruction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleHelloAsync(TClientDetail clientDetail, CancellationToken cancellationToken);
}

/// <summary>
/// Defines the contract for a Yaap server to handle client interactions and configure event handlers.
/// </summary>
public interface IYaapServer : IYaapServer<YaapClientDetail>
{
}

/// <summary>
/// Defines the contract for a Yaap server to handle client interactions and configure event handlers.
/// </summary>
public interface IYaapServer<TClientDetail, THelloResponse> where THelloResponse : notnull
{
    /// <summary>
    /// Handles a "Hello" instruction from a Yaap client.
    /// This method is invoked when a client sends a greeting or initialization message.
    /// </summary>
    /// <param name="clientDetail">Details of the Yaap client sending the instruction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<THelloResponse> HandleHelloAsync(TClientDetail clientDetail, CancellationToken cancellationToken);

    /// <summary>
    /// Handles a "Goodbye" notification from a Yaap client.
    /// This method is invoked when a client sends a shutdown or exit notification.
    /// </summary>
    /// <param name="clientDetail">Details of the Yaap client sending the notification.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleGoodbyeAsync(TClientDetail clientDetail, CancellationToken cancellationToken);
}