namespace AgentsMcpServer;

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Common;

using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.ChatCompletion;

using ModelContextProtocol;
using ModelContextProtocol.Protocol.Messages;
using ModelContextProtocol.Server;

using wsAgent.Core;

internal class Server(IServiceProvider sp) : Expert(sp)
{
    private readonly static ConcurrentDictionary<string, ClientWebSocket> _expertConnections = new();
    private readonly static ConcurrentDictionary<IPAddress, string> _expertIPAddresses = new();
    private readonly ConcurrentDictionary<string, HashSet<IMcpServer>> _connectedMcpClients = sp.GetRequiredService<ConcurrentDictionary<string, HashSet<IMcpServer>>>();
    private readonly IHttpContextAccessor _contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

    protected override bool PerformsIntroduction => false;

    protected override async Task<string> ProcessMessageAsync(WebSocket caller, string message, CancellationToken cancellationToken)
    {
        JsonElement jsonObject = JsonDocument.Parse(message).RootElement;
        var action = jsonObject.GetProperty("action").GetString();

        switch (action)
        {
            case "Introduce":
                await AddAgentAsync(jsonObject.GetProperty("detail"), cancellationToken);
                return JsonSerializer.Serialize(new { message = "Agent introduced" });

            // Add more cases for other actions

            default:
                return await base.ProcessMessageAsync(caller, message, cancellationToken);
        }
    }

    private async Task AddAgentAsync(JsonElement request, CancellationToken cancellationToken)
    {
        var name = Throws.IfNullOrWhiteSpace(request.GetProperty("Name").GetString());
        _log.AddingExpertNameToPanel(name);
        _log.RequestRequest(request);

        var agentClient = new ClientWebSocket();
        UriBuilder b = new UriBuilder(request.GetProperty("Secured").GetBoolean() ? "wss" : "ws",
            Throws.IfNullOrWhiteSpace(_contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()),
            request.GetProperty("CallbackPort").GetInt16(),
            "/ws/agent");
        await agentClient.ConnectAsync(b.Uri, cancellationToken);

        _expertConnections[name] = agentClient;
        _expertIPAddresses[_contextAccessor.HttpContext.Connection.RemoteIpAddress] = name;

        var description = request.GetProperty("Description").GetString();
        var mcpTool = McpServerTool.Create(AIFunctionFactory.Create(async (string action, ChatHistory prompt) =>
        {
            var response = await SendMessageAndGetResponseAsync(name, new { action, prompt }, cancellationToken);
            return response;
        }, name, description), new McpServerToolCreateOptions { Destructive = false, Idempotent = true, ReadOnly = true });

        var numExperts = ConnectedExperts.Count;
        ConnectedExperts = ConnectedExperts.Add(mcpTool);
        if (numExperts == ConnectedExperts.Count)
        {
            _log.McpToolNameAlreadyExistedInTheToolCollection(mcpTool.ProtocolTool.Name);
        }
        else
        {
            await SendToolsUpdatedNotificationAsync(cancellationToken);
        }
    }

    private async Task SendToolsUpdatedNotificationAsync(CancellationToken cancellationToken)
    {
        if (_connectedMcpClients.TryGetValue(NotificationMethods.ToolListChangedNotification, out var clients))
        {
            List<Task> notifications = [];
            foreach (var client in clients)
            {
                notifications.Add(client.SendNotificationAsync(NotificationMethods.ToolListChangedNotification, cancellationToken: cancellationToken));
            }

            await Task.WhenAll(notifications);
        }
    }

    internal static ImmutableHashSet<McpServerTool> ConnectedExperts { get; private set; } = [];

    private async Task<DataContent> SendMessageAndGetResponseAsync(string agentName, object message, CancellationToken cancellationToken)
    {
        if (!_expertConnections.TryGetValue(agentName, out ClientWebSocket? webSocket))
        {
            throw new InvalidOperationException($"No WebSocket connection found for agent {agentName}");
        }

        var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, cancellationToken);

        List<byte> fullResponse = [];

        var buffer = new Memory<byte>(new byte[1024 * 4]);
        do
        {
            var result = await webSocket.ReceiveAsync(buffer, cancellationToken);
            fullResponse.AddRange(buffer.ToArray().Take(result.Count));
            if (result.EndOfMessage)
            {
                break;
            }

            cancellationToken.ThrowIfCancellationRequested();
        } while (true);

        var resultStr = Encoding.UTF8.GetString([.. fullResponse], 0, fullResponse.Count);

        return new DataContent(new ReadOnlyMemory<byte>([.. fullResponse]), "application/json");
    }

    public async override Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        await base.HandleWebSocketAsync(webSocket, cancellationToken);

        // Remove the client from the list of tools
        if (_contextAccessor.HttpContext?.Connection.RemoteIpAddress is null)
        {
            _log.UnableToGetIPAddressForDisconnectingWebSocket();
            Debug.Fail(string.Empty);
            return;
        }

        if (!_expertIPAddresses.TryGetValue(_contextAccessor.HttpContext.Connection.RemoteIpAddress, out var expertName))
        {
            _log.NotAbleToFindAgentForConnectionIPWebSocketIp(_contextAccessor.HttpContext.Connection.RemoteIpAddress);
            Debug.Fail(string.Empty);
            return;
        }

        _log.AgentNameDisconnected(expertName);

        var removed = _expertConnections.TryRemove(expertName, out var _);
        Debug.Assert(removed);

        _log.RemovingAgentNameFromToolList(expertName);
        var tool = ConnectedExperts.First(i => i.ProtocolTool.Name == expertName);
        ConnectedExperts = ConnectedExperts.Remove(tool);
        _log.RemovedAgentNameFromToolListNotifyingClients(expertName);
        await SendToolsUpdatedNotificationAsync(default);
    }
}
