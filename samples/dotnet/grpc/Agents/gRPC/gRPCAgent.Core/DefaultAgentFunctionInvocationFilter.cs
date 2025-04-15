namespace gRPCAgent.Core;

using System.Text.Json;

using Common;

using Microsoft.Extensions.Logging;

using Orchestrator_gRPC;

internal class DefaultFunctionInvocationFilter(ILogger log, Orchestrator.OrchestratorClient signalr) : FunctionInvocationFilter(
        onStart: async context =>
        {
            var msg = $"Running {context.Function.Name} ({context.Function.Description}) ...";
            log.LogTrace(msg);
            await signalr.MessageAsync(new MessageRequest { Message = msg });
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
            await signalr.MessageAsync(new MessageRequest { Message = msg });
        },
        onAutoStart: async context =>
        {
            var msg = $"Running AUTO {context.Function.Name} ({context.Function.Description}) ...";
            log.LogTrace(msg);
            await signalr.MessageAsync(new MessageRequest { Message = msg });
        },
        onAutoEnd: async context =>
        {
            var msg = $"AUTO {context.Function.Name} completed.";
            log.MsgResultResult(msg, context.Result.ToString());
            await signalr.MessageAsync(new MessageRequest { Message = msg });
        }
)
{ }