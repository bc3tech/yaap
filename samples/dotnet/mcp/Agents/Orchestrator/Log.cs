namespace Orchestrator;
using Microsoft.Extensions.Logging;

static partial class Log
{

    [LoggerMessage(0, LogLevel.Debug, "Adding {expertName} to panel...")]
    internal static partial void AddingExpertNameToPanel(this ILogger logger, string expertName);

    [LoggerMessage(1, LogLevel.Trace, "Expert {expertName} added.")]
    internal static partial void ExpertExpertNameAdded(this ILogger logger, string expertName);

    [LoggerMessage(2, LogLevel.Debug, "gRPC message received: {GrpcMessage}")]
    internal static partial void GRPCMessageReceivedGrpcMessage(this ILogger logger, string GrpcMessage);

    [LoggerMessage(3, LogLevel.Warning, "Plugin {name} not found, but said Goodbye")]
    internal static partial void PluginNameNotFoundButSaidGoodbye(this ILogger? logger, string name);

    [LoggerMessage(4, LogLevel.Trace, "Adding Client {YaapClientName} to cache")]
    internal static partial void AddingClientYaapClientNameToCache(this ILogger logger, string YaapClientName);

    [LoggerMessage(5, LogLevel.Debug, "Client added to cache {YaapClientDetail}")]
    internal static partial void ClientAddedToCacheYaapClientDetail(this ILogger logger, Yaap.Core.Models.YaapClientDetail YaapClientDetail);

    [LoggerMessage(6, LogLevel.Trace, "Removing Client {YaapClientName} from cache")]
    internal static partial void RemovingClientYaapClientNameFromCache(this ILogger logger, string YaapClientName);

    [LoggerMessage(7, LogLevel.Debug, "Client {YaapClientName} removed from cache")]
    internal static partial void ClientYaapClientNameRemovedFromCache(this ILogger logger, string YaapClientName);

    [LoggerMessage(8, LogLevel.Information, "Tool list has changed")]
    internal static partial void ToolListHasChanged(this ILogger logger);
}
