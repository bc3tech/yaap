namespace Orchestrator_WS;

#pragma warning disable CS8019
using System;

using Microsoft.Extensions.Logging;
#pragma warning restore CS8019

static partial class Log
{

    [LoggerMessage(0, LogLevel.Debug, "Adding {expertName} to panel...")]
    internal static partial void AddingExpertNameToPanel(this ILogger logger, string expertName);

    [LoggerMessage(1, LogLevel.Trace, "Expert {expertName} added.")]
    internal static partial void ExpertExpertNameAdded(this ILogger logger, string expertName);

    [LoggerMessage(3, LogLevel.Warning, "Plugin {name} not found, but said goodbye")]
    internal static partial void PluginNameNotFoundButSaidGoodbye(this ILogger logger, string name);

    [LoggerMessage(4, LogLevel.Warning, "Failed to parse JSON message: {Message}")]
    internal static partial void FailedToParseJSONMessageMessage(this ILogger logger, Exception exception, string Message);

    [LoggerMessage(5, LogLevel.Warning, "An error occurred while processing the message: {Message}")]
    internal static partial void AnErrorOccurredWhileProcessingTheMessageMessage(this ILogger logger, Exception exception, string Message);

    [LoggerMessage(6, LogLevel.Warning, "Got a socket/cancellation error attempting to send a message to the {YaapClientName}, removing from Experts list via 'Goodbye'")]
    internal static partial void GotASocketCancellationErrorAttemptingToSendAMessageToTheYaapClientNameRemovingFromExpertsListViaGoodbye(this ILogger logger, Exception exception, string YaapClientName);
}
