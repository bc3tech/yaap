namespace Orchestrator_WS;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Messages;
using ModelContextProtocol.Protocol.Transport;

using wsAgent.Core;

internal class Orchestrator(IHttpContextAccessor _contextAccessor, IServiceProvider sp) : Expert(sp)
{
    private readonly static ConcurrentDictionary<IPAddress, ChatHistory> _threads = new();
    private IMcpClient? _mcpClient;

    protected override bool PerformsIntroduction { get; } = false;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _mcpClient ??= await McpClientFactory.CreateAsync(_services.GetRequiredService<IClientTransport>(), loggerFactory: _services.GetRequiredService<ILoggerFactory>(), cancellationToken: cancellationToken);
        _mcpClient.RegisterNotificationHandler(NotificationMethods.ToolListChangedNotification, async (notification, ct) =>
        {
            _log.ToolListHasChanged();
            await AddMcpToolsToSkAsync(cancellationToken);
        });

        await AddMcpToolsToSkAsync(cancellationToken);
    }

    private async Task AddMcpToolsToSkAsync(CancellationToken cancellationToken)
    {
        Debug.Assert(_mcpClient is not null);

        // Remove all the plugins with the `MCP_` prefix
        foreach (var p in _kernel.Plugins.Where(t => t.Name.StartsWith("MCP_")).ToArray())
        {
            _kernel.Plugins.Remove(p);
        }

        await foreach (var tool in _mcpClient.EnumerateToolsAsync(cancellationToken: cancellationToken))
        {
            _kernel.ImportPluginFromFunctions($"MCP_{tool.Name}", [
                _kernel.CreateFunctionFromMethod(
                    (string prompt) => SendMessageAndGetResponseAsync(tool.Name, ( "GetAnswer", prompt ), cancellationToken),
                    tool.Name, tool.Description,
                    parameters: [new("prompt") { IsRequired = true, ParameterType = typeof(string) }],
                    returnParameter: new KernelReturnParameterMetadata() { Description = "Prompt response as a JSON object or array to be inferred upon.", ParameterType = typeof(string) })]
            );
        }
    }

    protected override async Task<ChatHistory> GetAnswerInternalAsync(ChatHistory chatHistory, CancellationToken cancellationToken)
    {
        var thread = _threads.GetOrAdd(_contextAccessor.HttpContext!.Connection.RemoteIpAddress!, _ => []);
        thread.Add(chatHistory.Last());

        var updatedThread = await base.GetAnswerInternalAsync(thread, cancellationToken).ConfigureAwait(false);

        _threads[_contextAccessor.HttpContext!.Connection.RemoteIpAddress!] = updatedThread;
        return updatedThread;
    }

    private async Task<string> SendMessageAndGetResponseAsync(string agentName, (string action, string data) message, CancellationToken cancellationToken)
    {
        var thread = _threads[_contextAccessor.HttpContext!.Connection.RemoteIpAddress!];
        var toolResponse = await _mcpClient!.CallToolAsync(agentName, new Dictionary<string, object?> { ["action"] = message.action, ["prompt"] = thread }, cancellationToken: cancellationToken);

        var b64string = JsonSerializer.Deserialize<JsonElement>(toolResponse.Content.Single().Text!).GetProperty("uri").GetString()!;
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(b64string[(b64string.IndexOf(',') + 1)..]));
        var toolCompletion = JsonSerializer.Deserialize<ToolCompletion>(json)!;
        var updatedThread = toolCompletion.Completion;
        var updated = _threads.TryUpdate(_contextAccessor.HttpContext.Connection.RemoteIpAddress!, updatedThread, thread);
        Debug.Assert(updated);

        return updatedThread.Last().ToString();
    }

    record ToolCompletion([property: JsonPropertyName("completion")] ChatHistory Completion);
}
