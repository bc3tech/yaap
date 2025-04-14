
namespace Yaap.Client;

#pragma warning disable CS8019
using Microsoft.Extensions.Logging;
using System;
#pragma warning restore CS8019

static partial class Log
{

    [LoggerMessage(0, LogLevel.Trace, "Introducing myself to the YAAP server...")]
    internal static partial void IntroducingMyselfToTheYAAPServer(this ILogger logger);

    [LoggerMessage(1, LogLevel.Debug, "Introduction successful to YAAP server at {YaapServerEndpoint}")]
    internal static partial void IntroductionSuccessfulToYAAPServerAtYaapServerEndpoint(this ILogger logger, Uri YaapServerEndpoint);

    [LoggerMessage(2, LogLevel.Trace, "Saying goodbye to YAAP server...")]
    internal static partial void SayingGoodbyeToYAAPServer(this ILogger logger);

    [LoggerMessage(3, LogLevel.Debug, "Goodbye message sent to YAAP server.")]
    internal static partial void GoodbyeMessageSentToYAAPServer(this ILogger logger);

    [LoggerMessage(4, LogLevel.Debug, "Said goodbye to YAAP server as part of Dispose()")]
    internal static partial void SaidGoodbyeToYAAPServerAsPartOfDispose(this ILogger logger);

    [LoggerMessage(5, LogLevel.Trace, "Disposing of YAAP client")]
    internal static partial void DisposingOfYAAPClient(this ILogger logger);
}
