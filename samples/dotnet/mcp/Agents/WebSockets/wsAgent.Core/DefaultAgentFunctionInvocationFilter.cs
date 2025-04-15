namespace wsAgent.Core;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using Common;

using Microsoft.Extensions.Logging;

internal class DefaultFunctionInvocationFilter(ILogger log, ClientWebSocket signalr) : FunctionInvocationFilter(
        onStart: async context =>
        {
            var msg = $"Running {context.Function.Name} ({context.Function.Description}) ...";
#pragma warning disable EA0000 // Use source generated logging methods for improved performance
            log.LogTrace(msg);
#pragma warning restore EA0000 // Use source generated logging methods for improved performance
            await signalr.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($@"{{ ""message"": ""{msg}"" }}")), WebSocketMessageType.Text, true, CancellationToken.None);
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

            log.MsgResultResult(msg, serializedValue);
            await signalr.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($@"{{ ""message"": ""{msg}"" }}")), WebSocketMessageType.Text, true, CancellationToken.None);
        },
        onAutoStart: async context =>
        {
            var msg = $"Running AUTO {context.Function.Name} ({context.Function.Description}) ...";
#pragma warning disable EA0000 // Use source generated logging methods for improved performance
            log.LogTrace(msg);
#pragma warning restore EA0000 // Use source generated logging methods for improved performance
            await signalr.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($@"{{ ""message"": ""{msg}"" }}")), WebSocketMessageType.Text, true, CancellationToken.None);
        },
        onAutoEnd: async context =>
        {
            var msg = $"AUTO {context.Function.Name} completed.";
#pragma warning disable EA0001 // Perform message formatting in the body of the logging method
            log.MsgResultResult(msg, context.Result.ToString());
#pragma warning restore EA0001 // Perform message formatting in the body of the logging method
            await signalr.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($@"{{ ""message"": ""{msg}"" }}")), WebSocketMessageType.Text, true, CancellationToken.None);
        }
)
{ }