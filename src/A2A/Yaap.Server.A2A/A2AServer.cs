namespace Yaap.A2A.Server.AspNetCore;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Yaap.A2A.Core;
using Yaap.A2A.Core.Models;

public class A2AServer
{
    private readonly AgentCard _agentCard;
    private readonly TaskManager _taskManager;
    private readonly string _endpoint;

    public A2AServer(AgentCard agentCard, TaskManager taskManager, string endpoint = "/")
    {
        _agentCard = agentCard ?? throw new ArgumentNullException(nameof(agentCard));
        _taskManager = taskManager ?? throw new ArgumentNullException(nameof(taskManager));
        _endpoint = endpoint;
    }

    public async Task ProcessRequest(HttpContext context)
    {
        try
        {
            JSONRPCRequest? body = await JsonSerializer.DeserializeAsync<JSONRPCRequest>(context.Request.Body);

            JSONRPCResponse result = body switch
            {
                GetTaskRequest getTaskRequest => await _taskManager.OnGetTaskAsync(getTaskRequest, context.RequestAborted),
                SendTaskRequest sendTaskRequest => await _taskManager.OnSendTaskAsync(sendTaskRequest, context.RequestAborted),
                SendTaskStreamingRequest sendTaskStreamingRequest => await _taskManager.OnSendTaskSubscribeAsync(sendTaskStreamingRequest, context.RequestAborted),
                CancelTaskRequest cancelTaskRequest => await _taskManager.OnCancelTaskAsync(cancelTaskRequest, context.RequestAborted),
                SetTaskPushNotificationRequest setTaskPushNotificationRequest => await _taskManager.OnSetTaskPushNotificationAsync(setTaskPushNotificationRequest, context.RequestAborted),
                GetTaskPushNotificationRequest getTaskPushNotificationRequest => await _taskManager.OnGetTaskPushNotificationAsync(getTaskPushNotificationRequest, context.RequestAborted),
                TaskResubscriptionRequest taskResubscriptionRequest => await _taskManager.OnResubscribeToTaskAsync(taskResubscriptionRequest, context.RequestAborted),
                _ => throw new InvalidOperationException($"Unexpected request type: {body?.GetType()}")
            };

            await CreateResponse(context, result);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
        JSONRPCError error = exception switch
        {
            JsonException => new JSONParseError(),
            InvalidOperationException => new InvalidRequestError(),
            _ => new InternalError()
        };

        var response = new JSONRPCResponse<object>(null, error);

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private async Task CreateResponse(HttpContext context, object result)
    {
        if (result is IAsyncEnumerable<SendTaskStreamingResponse> asyncEnumerable)
        {
            context.Response.ContentType = "text/event-stream";
            await foreach (SendTaskStreamingResponse item in asyncEnumerable)
            {
                await context.Response.WriteAsync($"data: {JsonSerializer.Serialize(item)}\n\n");
                await context.Response.Body.FlushAsync();
            }
        }
        else if (result is JSONRPCResponse jsonResponse)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(jsonResponse));
        }
        else
        {
            throw new InvalidOperationException($"Unexpected result type: {result.GetType()}");
        }
    }
}
