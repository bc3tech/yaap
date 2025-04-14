namespace Orchestrator_gRPC;

using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using gRPCAgent.Core;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Messages;
using ModelContextProtocol.Protocol.Transport;

internal class Orchestrator(IClientTransport mcpTransport, Kernel kernel, IDistributedCache _threads, ILoggerFactory _loggerFactory, IConfiguration appConfig, IHttpClientFactory httpClientFactory, PromptExecutionSettings executionSettings, IHttpContextAccessor _contextAccessor) : Expert(appConfig, _loggerFactory, httpClientFactory, kernel, executionSettings)
{
    private IMcpClient? _mcpClient;

    protected override bool PerformsIntroduction => false;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _mcpClient ??= await McpClientFactory.CreateAsync(mcpTransport, loggerFactory: _loggerFactory, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        foreach (KernelPlugin? p in _kernel.Plugins.Where(t => t.Name.StartsWith("MCP_")).ToArray())
        {
            _kernel.Plugins.Remove(p);
        }

        await foreach (McpClientTool tool in _mcpClient.EnumerateToolsAsync(cancellationToken: cancellationToken))
        {
            _kernel.ImportPluginFromFunctions($"MCP_{tool.Name}", [
                _kernel.CreateFunctionFromMethod(
                    (string prompt) => GetAnswerAsync(tool.Name, prompt , cancellationToken),
                    tool.Name, tool.Description,
                    parameters: [new("prompt") { IsRequired = true, ParameterType = typeof(string) }],
                    returnParameter: new KernelReturnParameterMetadata() { Description = "Prompt response as a JSON object or array to be inferred upon.", ParameterType = typeof(string) })]
            );
        }
    }

    record ToolCompletion([property: JsonPropertyName("thread")] ChatHistory Thread);

    private async Task<string> GetAnswerAsync(string agentName, string prompt, CancellationToken cancellationToken)
    {
        ChatHistory thread = await GetThreadAsync(_contextAccessor.HttpContext!.Connection.RemoteIpAddress!, cancellationToken) ?? new ChatHistory(prompt, AuthorRole.User);
        ModelContextProtocol.Protocol.Types.CallToolResponse toolResponse = await _mcpClient!.CallToolAsync(agentName, new Dictionary<string, object?> { ["thread"] = thread }, cancellationToken: cancellationToken);

        var b64string = JsonSerializer.Deserialize<JsonElement>(toolResponse.Content.Single().Text!).GetProperty("uri").GetString()!;
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(b64string[(b64string.IndexOf(',') + 1)..]));
        ToolCompletion toolCompletion = JsonSerializer.Deserialize<ToolCompletion>(json)!;
        ChatHistory updatedThread = toolCompletion.Thread;
        await UpdateThreadAsync(_contextAccessor.HttpContext.Connection.RemoteIpAddress!, updatedThread, cancellationToken);

        return updatedThread.Last().ToString();
    }

    private async Task<ChatHistory?> GetThreadAsync(IPAddress threadId, CancellationToken cancellationToken)
    {
        var thread = await _threads.GetAsync(threadId.ToString(), cancellationToken);
        return thread is null ? null : JsonSerializer.Deserialize<ChatHistory>(thread.AsSpan());
    }

    private Task UpdateThreadAsync(IPAddress threadId, ChatHistory chatHistory, CancellationToken cancellationToken) => _threads.SetAsync(threadId.ToString(), JsonSerializer.SerializeToUtf8Bytes(chatHistory), cancellationToken);
}
