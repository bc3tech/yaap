namespace AgentsMcpServer;

using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Common;

using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

using Grpc.Expert;
using Grpc.Net.Client;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.SemanticKernel.ChatCompletion;

using ModelContextProtocol;
using ModelContextProtocol.Protocol.Messages;
using ModelContextProtocol.Server;

using Yaap.Models;
using Yaap.Server;

internal class Server(ConcurrentDictionary<string, HashSet<IMcpServer>> _subscriptions, IDistributedCache cache, ILoggerFactory loggerFactory) : BaseYaapServer<Empty>(cache, loggerFactory)
{
    protected override async Task<Empty> HandleHelloCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
    {
        await AddAgentAsync(clientDetail, cancellationToken).ConfigureAwait(false);
        return new();
    }

    private async Task AddAgentAsync(YaapClientDetail request, CancellationToken cancellationToken)
    {
        var name = Throws.IfNullOrWhiteSpace(request.Name);
        this.Log?.AddingExpertNameToPanel(name);
        this.Log?.RequestRequest(request);

        var mcpTool = McpServerTool.Create(AIFunctionFactory.Create(async (ChatHistory thread) =>
        {
            var response = await SendMessageAndGetResponseAsync(name, thread, cancellationToken);
            return response;
        }, name, request.Description), new McpServerToolCreateOptions { Destructive = false, Idempotent = true, ReadOnly = true });

        if (!_connectedExperts.Add(mcpTool))
        {
            this.Log?.McpToolNameAlreadyExistedInTheToolCollection(mcpTool.ProtocolTool.Name);
        }
        else
        {
            await SendToolsUpdatedNotificationAsync(cancellationToken);
        }
    }

    private async Task SendToolsUpdatedNotificationAsync(CancellationToken cancellationToken)
    {
        if (_subscriptions.TryGetValue(NotificationMethods.ToolListChangedNotification, out HashSet<IMcpServer>? clients))
        {
            List<Task> notifications = [];
            foreach (IMcpServer client in clients)
            {
                notifications.Add(client.SendNotificationAsync(NotificationMethods.ToolListChangedNotification, cancellationToken: cancellationToken));
            }

            await Task.WhenAll(notifications);
        }
    }

    private static readonly HashSet<McpServerTool> _connectedExperts = [];
    internal static IReadOnlySet<McpServerTool> ConnectedExperts => _connectedExperts;

    private async Task<DataContent> SendMessageAndGetResponseAsync(string agentName, ChatHistory thread, CancellationToken cancellationToken)
    {
        var yaapClientDetailStr = await this.ClientCache.GetAsync(agentName, cancellationToken);
        if (yaapClientDetailStr is null)
        {
            throw new InvalidOperationException($"No connection found for agent {agentName}");
        }

        YaapClientDetail yaapClientDetail = JsonSerializer.Deserialize<YaapClientDetail>(yaapClientDetailStr)!;
        var client = new Expert.ExpertClient(GrpcChannel.ForAddress(yaapClientDetail.CallbackUrl!));
        Grpc.Models.AnswerResponse r = await client.GetAnswerAsync(new() { ChatHistory = ByteString.CopyFrom(JsonSerializer.SerializeToUtf8Bytes(thread)) }, cancellationToken: cancellationToken);

        thread.AddAssistantMessage(r.Completion);
        return new DataContent(JsonSerializer.SerializeToUtf8Bytes(new { thread }), "application/json");
    }
}
