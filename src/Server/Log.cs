
namespace Yaap.Server;

using A2A.Models;

using Microsoft.Extensions.Logging;

static partial class Log
{

    [LoggerMessage(1, LogLevel.Trace, "Adding Client {YaapClientName} to cache")]
    internal static partial void AddingClientYaapClientNameToCache(this ILogger logger, string YaapClientName);

    [LoggerMessage(2, LogLevel.Debug, "Client added to cache {AgentCard}")]
    internal static partial void ClientAddedToCacheYaapClientDetail(this ILogger logger, AgentCard AgentCard);

    [LoggerMessage(3, LogLevel.Trace, "Removing Client {YaapClientName} from cache")]
    internal static partial void RemovingClientYaapClientNameFromCache(this ILogger logger, string YaapClientName);

    [LoggerMessage(4, LogLevel.Debug, "Client {YaapClientName} removed from cache")]
    internal static partial void ClientYaapClientNameRemovedFromCache(this ILogger logger, string YaapClientName);
}
