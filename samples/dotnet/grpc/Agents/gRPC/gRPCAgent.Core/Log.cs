
namespace gRPCAgent.Core;

using System;

using Microsoft.Extensions.Logging;

static partial class Log
{

    [LoggerMessage(0, LogLevel.Trace, "{msg} Result: {result}")]
    internal static partial void MsgResultResult(this ILogger logger, string msg, string result);

    [LoggerMessage(1, LogLevel.Debug, "Introducing myself...")]
    internal static partial void IntroducingMyself(this ILogger logger);

    [LoggerMessage(2, LogLevel.Debug, "Prompt handled. Response: {promptResponse}")]
    internal static partial void PromptHandledResponsePromptResponse(this ILogger logger, Microsoft.SemanticKernel.FunctionResult promptResponse);

    [LoggerMessage(3, LogLevel.Error, "Error handling prompt: {prompt}")]
    internal static partial void ErrorHandlingPromptPrompt(this ILogger logger, Exception exception, string prompt);

    [LoggerMessage(4, LogLevel.Warning, "Responses Throttled! Waiting {retryAfter} seconds to try again...")]
    internal static partial void ResponsesThrottledWaitingRetryAfterSecondsToTryAgain(this ILogger logger, string retryAfter);

    [LoggerMessage(5, LogLevel.Error, "Max retries exceeded.")]
    internal static partial void MaxRetriesExceeded(this ILogger logger, Exception exception);

    [LoggerMessage(6, LogLevel.Information, "Awaiting question...")]
    internal static partial void AwaitingQuestion(this ILogger logger);

    [LoggerMessage(7, LogLevel.Information, "Saying goodbye to Orchestrator...")]
    internal static partial void SayingGoodbyeToOrchestrator(this ILogger logger);

    [LoggerMessage(8, LogLevel.Debug, "AzureOpenAIEndpoint: {AzureOpenAIEndpoint}")]
    internal static partial void AzureOpenAIEndpointAzureOpenAIEndpoint(this ILogger logger, string? AzureOpenAIEndpoint);

    [LoggerMessage(9, LogLevel.Debug, "Using AzureOpenAIKey")]
    internal static partial void UsingAzureOpenAIKey(this ILogger logger);

    [LoggerMessage(10, LogLevel.Debug, "Using Identity")]
    internal static partial void UsingIdentity(this ILogger logger);

    [LoggerMessage(11, LogLevel.Debug, "OpenAIEndpoint: {OpenAIEndpoint}")]
    internal static partial void OpenAIEndpointOpenAIEndpoint(this ILogger logger, string? OpenAIEndpoint);

    [LoggerMessage(12, LogLevel.Trace, "Introducing myself to the YAAP server...")]
    internal static partial void IntroducingMyselfToTheYAAPServer(this ILogger logger);

    [LoggerMessage(13, LogLevel.Debug, "Introduction successful to YAAP server at {YaapServerEndpoint}")]
    internal static partial void IntroductionSuccessfulToYAAPServerAtYaapServerEndpoint(this ILogger logger, Uri YaapServerEndpoint);

    [LoggerMessage(14, LogLevel.Trace, "Saying goodbye to YAAP server...")]
    internal static partial void SayingGoodbyeToYAAPServer(this ILogger logger);

    [LoggerMessage(15, LogLevel.Debug, "Goodbye message sent to YAAP server.")]
    internal static partial void GoodbyeMessageSentToYAAPServer(this ILogger logger);

    [LoggerMessage(16, LogLevel.Debug, "Said goodbye to YAAP server as part of Dispose()")]
    internal static partial void SaidGoodbyeToYAAPServerAsPartOfDispose(this ILogger logger);

    [LoggerMessage(17, LogLevel.Trace, "Disposing of YAAP client")]
    internal static partial void DisposingOfYAAPClient(this ILogger logger);
}
