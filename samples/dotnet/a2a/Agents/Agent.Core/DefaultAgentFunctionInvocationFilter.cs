using wsAgent.Core;

namespace Agent.Core;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using Common;

using Microsoft.Extensions.Logging;

internal class DefaultFunctionInvocationFilter(ILogger log, ClientWebSocket signalr) : FunctionInvocationFilter(
        onStart: async context =>
        {
            var msg = $"Running {context.Function.Name} ({context.Function.Description}) ...";
            log.LogTrace(msg);
            signalr.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($@"{{ ""message"": ""{msg}"" }}")), WebSocketMessageType.Text, true, CancellationToken.None);
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
            signalr.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($@"{{ ""message"": ""{msg}"" }}")), WebSocketMessageType.Text, true, CancellationToken.None);
        },
        onAutoStart: async context =>
        {
            var msg = $"Running AUTO {context.Function.Name} ({context.Function.Description}) ...";
            log.LogTrace(msg);
            signalr.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($@"{{ ""message"": ""{msg}"" }}")), WebSocketMessageType.Text, true, CancellationToken.None);
        },
        onAutoEnd: async context =>
        {
            var msg = $"AUTO {context.Function.Name} completed.";
            log.MsgResultResult(msg, context.Result.ToString());
            signalr.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($@"{{ ""message"": ""{msg}"" }}")), WebSocketMessageType.Text, true, CancellationToken.None);
        }
)
{ }