namespace AgentsMcpServer;

using Microsoft.Extensions.Logging;

using Yaap.Core.Models;

static partial class Log
{

    [LoggerMessage(0, LogLevel.Debug, "Adding {expertName} to panel...")]
    internal static partial void AddingExpertNameToPanel(this ILogger logger, string expertName);

    [LoggerMessage(1, LogLevel.Trace, "Expert {expertName} added.")]
    internal static partial void ExpertExpertNameAdded(this ILogger logger, string expertName);

    [LoggerMessage(2, LogLevel.Debug, "Request: {Request}")]
    internal static partial void RequestRequest(this ILogger logger, YaapClientDetail Request);

    [LoggerMessage(3, LogLevel.Warning, "'{McpToolName}' already existed in the tool collection.")]
    internal static partial void McpToolNameAlreadyExistedInTheToolCollection(this ILogger logger, string McpToolName);

    [LoggerMessage(4, LogLevel.Warning, "Unable to get IP address for disconnecting WebSocket")]
    internal static partial void UnableToGetIPAddressForDisconnectingWebSocket(this ILogger logger);

    [LoggerMessage(5, LogLevel.Error, "Not able to find agent for connection IP {WebSocketIp}")]
    internal static partial void NotAbleToFindAgentForConnectionIPWebSocketIp(this ILogger logger, System.Net.IPAddress WebSocketIp);

    [LoggerMessage(6, LogLevel.Information, "{AgentName} disconnected.")]
    internal static partial void AgentNameDisconnected(this ILogger logger, string AgentName);

    [LoggerMessage(7, LogLevel.Trace, "Removing {AgentName} from tool list...")]
    internal static partial void RemovingAgentNameFromToolList(this ILogger logger, string AgentName);

    [LoggerMessage(8, LogLevel.Debug, "Removed {AgentName} from tool list. Notifying clients...")]
    internal static partial void RemovedAgentNameFromToolListNotifyingClients(this ILogger logger, string AgentName);

    [LoggerMessage(9, LogLevel.Debug, "gRPC message received: {GrpcMessage}")]
    internal static partial void GRPCMessageReceivedGrpcMessage(this ILogger logger, string GrpcMessage);
}
