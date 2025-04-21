namespace ConsoleApp;

#pragma warning disable CS8019
using System;

using Microsoft.Extensions.Logging;
#pragma warning restore CS8019

static partial class Log
{

    [LoggerMessage(0, LogLevel.Debug, "{expertName} is now available.")]
    internal static partial void ExpertNameIsNowAvailable(this ILogger logger, string expertName);

    [LoggerMessage(1, LogLevel.Debug, "{expertName} has disconnected.")]
    internal static partial void ExpertNameHasDisconnected(this ILogger logger, string expertName);

    [LoggerMessage(2, LogLevel.Information, "Connecting to server...")]
    internal static partial void ConnectingToServer(this ILogger logger);

    [LoggerMessage(3, LogLevel.Information, "Time to answer: {tta}")]
    internal static partial void TimeToAnswerTta(this ILogger logger, TimeSpan tta);
}
