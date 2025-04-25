namespace Yaap.A2A.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Yaap.A2A.Core.Models;

/// <summary>
/// Abstract base class for managing tasks.
/// </summary>
public abstract class TaskManager
{
    /// <summary>
    /// Handles the retrieval of a task.
    /// </summary>
    /// <param name="request">The request to get a task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the task.</returns>
    public abstract Task<GetTaskResponse> OnGetTaskAsync(GetTaskRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the cancellation of a task.
    /// </summary>
    /// <param name="request">The request to cancel a task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the cancellation result.</returns>
    public abstract Task<CancelTaskResponse> OnCancelTaskAsync(CancelTaskRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the sending of a task.
    /// </summary>
    /// <param name="request">The request to send a task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the send result.</returns>
    public abstract Task<SendTaskResponse> OnSendTaskAsync(SendTaskRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the subscription to task streaming.
    /// </summary>
    /// <param name="request">The request to subscribe to task streaming.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the streaming result.</returns>
    public abstract Task<SendTaskStreamingResponse> OnSendTaskSubscribeAsync(SendTaskStreamingRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the setting of task push notifications.
    /// </summary>
    /// <param name="request">The request to set task push notifications.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the push notification result.</returns>
    public abstract Task<SetTaskPushNotificationResponse> OnSetTaskPushNotificationAsync(SetTaskPushNotificationRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the retrieval of task push notifications.
    /// </summary>
    /// <param name="request">The request to get task push notifications.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the push notification result.</returns>
    public abstract Task<GetTaskPushNotificationResponse> OnGetTaskPushNotificationAsync(GetTaskPushNotificationRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the resubscription to a task.
    /// </summary>
    /// <param name="request">The request to resubscribe to a task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the resubscription result.</returns>
    public abstract Task<SendTaskStreamingResponse> OnResubscribeToTaskAsync(TaskResubscriptionRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// In-memory implementation of the TaskManager.
/// </summary>
public class InMemoryTaskManager(ILogger<InMemoryTaskManager> logger) : TaskManager
{
    private readonly ConcurrentDictionary<string, AgentTask> _tasks = new();
    private readonly ConcurrentDictionary<string, PushNotificationConfig> _pushNotificationInfos = new();

    /// <summary>
    /// Handles the retrieval of a task.
    /// </summary>
    /// <param name="request">The request to get a task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the task.</returns>
    public override Task<GetTaskResponse> OnGetTaskAsync(GetTaskRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting task {request.Params.Id}");
        if (_tasks.TryGetValue(request.Params.Id, out AgentTask? task))
        {
            AgentTask taskResult = AppendTaskHistory(task, request.Params.HistoryLength);
            return Task.FromResult(new GetTaskResponse(request.Id, taskResult));
        }

        return Task.FromResult(new GetTaskResponse(request.Id, Error: new TaskNotFoundError()));
    }

    /// <summary>
    /// Handles the cancellation of a task.
    /// </summary>
    /// <param name="request">The request to cancel a task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the cancellation result.</returns>
    public override async Task<CancelTaskResponse> OnCancelTaskAsync(CancelTaskRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Cancelling task {request.Params.Id}");
        if (_tasks.TryGetValue(request.Params.Id, out AgentTask? task))
        {
            return new CancelTaskResponse(request.Id, Error: new TaskNotCancelableError());
        }

        return new CancelTaskResponse(request.Id, Error: new TaskNotFoundError());
    }

    /// <summary>
    /// Handles the sending of a task.
    /// </summary>
    /// <param name="request">The request to send a task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the send result.</returns>
    public override async Task<SendTaskResponse> OnSendTaskAsync(SendTaskRequest request, CancellationToken cancellationToken) =>
        // Implementation for sending a task
        throw new NotImplementedException();

    /// <summary>
    /// Handles the subscription to task streaming.
    /// </summary>
    /// <param name="request">The request to subscribe to task streaming.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the streaming result.</returns>
    public override async Task<SendTaskStreamingResponse> OnSendTaskSubscribeAsync(SendTaskStreamingRequest request, CancellationToken cancellationToken) =>
        // Implementation for subscribing to task streaming
        throw new NotImplementedException();

    /// <summary>
    /// Handles the setting of task push notifications.
    /// </summary>
    /// <param name="request">The request to set task push notifications.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the push notification result.</returns>
    public override async Task<SetTaskPushNotificationResponse> OnSetTaskPushNotificationAsync(SetTaskPushNotificationRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Setting task push notification {request.Params.Id}");
        try
        {
            _pushNotificationInfos[request.Params.Id] = request.Params.PushNotificationConfig;
            return new SetTaskPushNotificationResponse(request.Id, request.Params);
        }
        catch (Exception e)
        {
            logger.LogError($"Error while setting push notification info: {e}");
            return new SetTaskPushNotificationResponse(request.Id, Error: new InternalError("An error occurred while setting push notification info"));
        }
    }

    /// <summary>
    /// Handles the retrieval of task push notifications.
    /// </summary>
    /// <param name="request">The request to get task push notifications.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the push notification result.</returns>
    public override async Task<GetTaskPushNotificationResponse> OnGetTaskPushNotificationAsync(GetTaskPushNotificationRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting task push notification {request.Params.Id}");
        try
        {
            if (_pushNotificationInfos.TryGetValue(request.Params.Id, out PushNotificationConfig? notificationInfo))
            {
                return new GetTaskPushNotificationResponse(request.Id, new TaskPushNotificationConfig(request.Params.Id, notificationInfo));
            }

            return new GetTaskPushNotificationResponse(request.Id, Error: new TaskNotFoundError());
        }
        catch (Exception e)
        {
            logger.LogError($"Error while getting push notification info: {e}");
            return new GetTaskPushNotificationResponse(request.Id, Error: new InternalError("An error occurred while getting push notification info"));
        }
    }

    /// <summary>
    /// Handles the resubscription to a task.
    /// </summary>
    /// <param name="request">The request to resubscribe to a task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response containing the resubscription result.</returns>
    public override async Task<SendTaskStreamingResponse> OnResubscribeToTaskAsync(TaskResubscriptionRequest request, CancellationToken cancellationToken) =>
        // Implementation for resubscribing to a task
        throw new NotImplementedException();

    /// <summary>
    /// Appends the task history.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="historyLength">The length of the history.</param>
    /// <returns>The task with appended history.</returns>
    private static AgentTask AppendTaskHistory(AgentTask task, int? historyLength)
    {
        AgentTask newTask = task with
        {
            History = historyLength.HasValue && historyLength.Value > 0
                ? [.. task.History?.TakeLast(historyLength.Value) ?? []]
                : []
        };

        return newTask;
    }
}
