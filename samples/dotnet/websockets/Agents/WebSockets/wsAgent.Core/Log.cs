namespace wsAgent.Core;

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
}
