namespace Orchestrator_WS;

using Microsoft.Extensions.Logging;

static partial class Log
{

    [LoggerMessage(0, LogLevel.Debug, "Adding {expertName} to panel...")]
    internal static partial void AddingExpertNameToPanel(this ILogger logger, string expertName);

    [LoggerMessage(1, LogLevel.Trace, "Expert {expertName} added.")]
    internal static partial void ExpertExpertNameAdded(this ILogger logger, string expertName);

    [LoggerMessage(2, LogLevel.Information, "Tool list has changed")]
    internal static partial void ToolListHasChanged(this ILogger logger);
}
