namespace Orchestrator;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Agent.Core;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.SemanticKernel;

using Orchestrator_WS;

using Yaap.Core.Models;
using Yaap.Server.Abstractions;

internal class Worker(Kernel _kernel, PromptExecutionSettings promptSettings, IDistributedCache cache, ILoggerFactory loggerFactory) : BaseYaapServer(cache, loggerFactory)
{
    private readonly ILogger<Worker> _log = loggerFactory.CreateLogger<Worker>();

    private static readonly ConcurrentDictionary<string, ClientWebSocket> _expertConnections = new();

    internal Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken) => AIHelpers.HandleWebSocketAsync(webSocket, ProcessMessageAsync, cancellationToken);

    private async Task<string?> ProcessMessageAsync(WebSocket caller, WebSocketReceiveResult raw, string message, CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested && !string.IsNullOrWhiteSpace(message))
        {
            try
            {
                JsonElement jsonObject = JsonDocument.Parse(message).RootElement;
                var action = jsonObject.GetProperty("action").GetString();

                switch (action)
                {
                    case "Hello":
                        YaapClientDetail clientDetail = jsonObject.GetProperty("detail").Deserialize<YaapClientDetail>()!;

                        if (clientDetail.CallbackUrl is null)
                        {
                            throw new ArgumentNullException(nameof(clientDetail.CallbackUrl), "Callback URL is required");
                        }

                        await HandleHelloAsync(clientDetail!, cancellationToken).ConfigureAwait(false);
                        return JsonSerializer.Serialize(new { message = "Agent acknowledged" });

                    case "Goodbye":
                        await HandleGoodbyeAsync(jsonObject.GetProperty("detail").Deserialize<YaapClientDetail>()!, cancellationToken).ConfigureAwait(false);
                        return JsonSerializer.Serialize(new { message = "Agent removed" });

                    // Add more cases for other actions

                    default:
                        return await AIHelpers.ProcessMessageAsync(caller, raw, message, _kernel, promptSettings, cancellationToken, _log).ConfigureAwait(false);
                }
            }
            catch (JsonException e)
            {
                _log.FailedToParseJSONMessageMessage(e, message);
                return JsonSerializer.Serialize(new { error = "Invalid JSON format" });
            }
            catch (Exception e)
            {
                _log.AnErrorOccurredWhileProcessingTheMessageMessage(e, message);
                return JsonSerializer.Serialize(new { error = "Internal server error" });
            }
        }

        return string.Empty;
    }

    private readonly ArraySegment<byte> _buffer = new(new byte[1024 * 4]);

    protected override async Task HandleHelloCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
    {
        var socket = _expertConnections.AddOrUpdate(clientDetail.Name, new ClientWebSocket(), (_, _) => new ClientWebSocket());
        await socket.ConnectAsync(clientDetail.CallbackUrl!, cancellationToken).ConfigureAwait(false);

        _kernel.ImportPluginFromFunctions(clientDetail.Name, [
            _kernel.CreateFunctionFromMethod(
                async (string prompt) => {
                    var response = await SendMessageAndGetResponseAsync(clientDetail, new { action = "GetAnswer", prompt }, cancellationToken).ConfigureAwait(false);
                    return response;
                },
                clientDetail.Name, clientDetail.Description,
                [new ("prompt") { IsRequired = true, ParameterType = typeof(string) }],
                new () { Description = "Prompt response as a JSON object or array to be inferred upon.", ParameterType = typeof(string) })
            ]);
    }

    private async Task<string> SendMessageAndGetResponseAsync(YaapClientDetail clientDetail, object message, CancellationToken cancellationToken)
    {
        var webSocket = _expertConnections[clientDetail.Name];
        try
        {
            await AIHelpers.SendMessageAsync(webSocket, JsonSerializer.Serialize(message), cancellationToken).ConfigureAwait(false);

            (var socketResponse, var bytes) = await AIHelpers.ReceiveResponseAsync(webSocket, _buffer, cancellationToken).ConfigureAwait(false);
            if (socketResponse.CloseStatus is not null)
            {
                throw new WebSocketException($"WebSocket closed unexpectedly (Status: {socketResponse.CloseStatus.Value})");
            }

            var response = Encoding.UTF8.GetString([.. bytes], 0, bytes.Length);
            return response;
        }
        catch (Exception e) when (e is OperationCanceledException or WebSocketException)
        {
            _log.GotASocketCancellationErrorAttemptingToSendAMessageToTheYaapClientNameRemovingFromExpertsListViaGoodbye(e, clientDetail.Name);
            await HandleGoodbyeAsync(clientDetail, cancellationToken);

            throw;
        }
    }

    protected override Task HandleGoodbyeCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
    {
        if (!_kernel.Plugins.TryGetPlugin(clientDetail.Name, out KernelPlugin? plugin) || plugin is null)
        {
            _log.PluginNameNotFoundButSaidGoodbye(clientDetail.Name);
            Debug.Fail(null);
            return Task.CompletedTask;
        }

        _kernel.Plugins.Remove(plugin);
        return Task.CompletedTask;
    }
}
