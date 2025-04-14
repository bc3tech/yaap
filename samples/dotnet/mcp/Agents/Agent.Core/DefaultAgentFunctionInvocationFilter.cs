namespace Agent.Core;

using System.Text.Json;

using Common;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

public class DefaultFunctionInvocationFilter(ILogger log, HubConnection signalr) : FunctionInvocationFilter(
        onStart: async context =>
        {
            var msg = $"Running {context.Function.Name} ({context.Function.Description}) ...";
            log.LogTrace(msg);
            await signalr.SendAsync(Constants.SignalR.Functions.PostStatus, msg);
        },
        onEnd: async context =>
        {
            var msg = $"{context.Function.Name} completed.";
            string serializedValue;
            try
            {
                serializedValue = JsonSerializer.Serialize(context.Result.GetValue<object>(), context.Result.ValueType!);
            }
            catch
            {
                serializedValue = context.Result.ToString();
            }

            log.LogTrace("{msg} Result: {result}", msg, serializedValue);
            await signalr.SendAsync(Constants.SignalR.Functions.PostStatus, msg);
        },
        onAutoStart: async context =>
        {
            var msg = $"Running AUTO {context.Function.Name} ({context.Function.Description}) ...";
            log.LogTrace(msg);
            await signalr.SendAsync(Constants.SignalR.Functions.PostStatus, msg);
        },
        onAutoEnd: async context =>
        {
            var msg = $"AUTO {context.Function.Name} completed.";
            log.LogTrace("{msg} Result: {result}", msg, context.Result.ToString());
            await signalr.SendAsync(Constants.SignalR.Functions.PostStatus, msg);
        }
)
{ }