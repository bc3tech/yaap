namespace Agent.Core;

#pragma warning disable CS8019
using System;

using Microsoft.Extensions.Logging;
#pragma warning restore CS8019

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

    [LoggerMessage(8, LogLevel.Debug, "AzureOpenAIEndpoint: {AzureOpenAIEndpoint}")]
    internal static partial void AzureOpenAIEndpointAzureOpenAIEndpoint(this ILogger logger, string? AzureOpenAIEndpoint);

    [LoggerMessage(9, LogLevel.Debug, "Using AzureOpenAIKey")]
    internal static partial void UsingAzureOpenAIKey(this ILogger logger);

    [LoggerMessage(10, LogLevel.Debug, "Using Identity")]
    internal static partial void UsingIdentity(this ILogger logger);

    [LoggerMessage(11, LogLevel.Debug, "OpenAIEndpoint: {OpenAIEndpoint}")]
    internal static partial void OpenAIEndpointOpenAIEndpoint(this ILogger logger, string? OpenAIEndpoint);

    [LoggerMessage(12, LogLevel.Debug, "Got exception during websocket receive; assuming \"dirty\" close")]
    internal static partial void GotExceptionDuringWebsocketReceiveAssumingDirtyClose(this ILogger logger, Exception exception);

    [LoggerMessage(13, LogLevel.Error, "Hit exception when trying to close the Websocket connection; disregarding")]
    internal static partial void HitExceptionWhenTryingToCloseTheWebsocketConnectionDisregarding(this ILogger logger, Exception exception);

    [LoggerMessage(14, LogLevel.Error, "Error trying to say Hello to YAAP server at {YaapServerEndpoint}")]
    internal static partial void ErrorTryingToSayHelloToYAAPServerAtYaapServerEndpoint(this ILogger logger, Exception exception, Uri YaapServerEndpoint);

    [LoggerMessage(15, LogLevel.Error, "Error trying to say Goodbye to YAAP server at {YaapServerEndpoint}")]
    internal static partial void ErrorTryingToSayGoodbyeToYAAPServerAtYaapServerEndpoint(this ILogger logger, Exception exception, Uri YaapServerEndpoint);

    [LoggerMessage(16, LogLevel.Warning, "Bad ack for 'Hello' from YAAP server: {AckPayload}")]
    internal static partial void BadAckForHelloFromYAAPServerAckPayload(this ILogger logger, System.Text.Json.JsonElement AckPayload);
}
