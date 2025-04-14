namespace gRPCAgent.Core;
using System;
using System.Collections.Immutable;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Google.Protobuf;

using Grpc.Core;
using Grpc.Models;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

public static class AIHelpers
{
    public static async Task<T> ExecuteWithThrottleHandlingAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken, int maxRetries = 10, ILogger? log = null)
    {
        Exception? lastException = null;
        for (var i = 0; i < maxRetries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await operation().ConfigureAwait(false);
            }
            catch (HttpOperationException ex)
            {
                lastException = ex;
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests && ex.InnerException is Azure.RequestFailedException rex)
                {
                    Azure.Response? resp = rex.GetRawResponse();
                    if (resp?.Headers.TryGetValue("Retry-After", out var waitTime) is true)
                    {
                        log?.ResponsesThrottledWaitingRetryAfterSecondsToTryAgain(waitTime);
                        await Task.Delay(TimeSpan.FromSeconds(int.Parse(waitTime)), cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        log?.MaxRetriesExceeded(lastException);
        throw lastException!;
    }

    public static async Task ExecuteWithThrottleHandlingAsync(Func<Task> operation, CancellationToken cancellationToken, int maxRetries = 10, ILogger? log = null)
    {
        Exception? lastException = null;
        for (var i = 0; i < maxRetries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await operation().ConfigureAwait(false);
                return;
            }
            catch (HttpOperationException ex)
            {
                lastException = ex;
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests && ex.InnerException is Azure.RequestFailedException rex)
                {
                    Azure.Response? resp = rex.GetRawResponse();
                    if (resp?.Headers.TryGetValue("Retry-After", out var waitTime) is true)
                    {
                        log?.ResponsesThrottledWaitingRetryAfterSecondsToTryAgain(waitTime);
                        await Task.Delay(TimeSpan.FromSeconds(int.Parse(waitTime)), cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        log?.MaxRetriesExceeded(lastException);
        throw lastException!;
    }

    public static Task SendMessageAsync(WebSocket webSocket, string message, CancellationToken cancellationToken) => webSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, cancellationToken);

    public static async Task<(WebSocketReceiveResult lastReceiveResult, ImmutableArray<byte> responseBytes)> ReceiveResponseAsync(WebSocket webSocket, ArraySegment<byte> buffer, CancellationToken cancellationToken)
    {
        List<byte> webSocketResponseBytes = [];
        while (true)
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
            webSocketResponseBytes.AddRange(buffer[..result.Count]);
            if (result.EndOfMessage)
            {
                return (result, webSocketResponseBytes.ToImmutableArray());
            }
        }
    }

    public static Task GetAnswerStreamingAsync(Kernel kernel, PromptExecutionSettings promptSettings, ByteString chatHistoryBytes, IServerStreamWriter<StreamResponse> responseStream, CancellationToken cancellationToken, ILogger? log = null)
    {
        var completionService = (IChatCompletionService)kernel.Services.GetService(typeof(IChatCompletionService))!;
        ChatHistory chatHistory = JsonSerializer.Deserialize<ChatHistory>(chatHistoryBytes.Span)!;

        return ExecuteWithThrottleHandlingAsync(async () =>
        {
            string response;
            try
            {
                await foreach (StreamingKernelContent token in completionService.GetStreamingChatMessageContentsAsync(chatHistory, promptSettings, kernel, cancellationToken))
                {
                    await responseStream.WriteAsync(new() { Token = token.ToString() });
                }
            }
            catch (Exception ex)
            {
                log?.ErrorHandlingPromptPrompt(ex, chatHistory.Last());

                response = JsonSerializer.Serialize(ex.Message);
            }
        }, cancellationToken);
    }

    public static async Task<string> GetAnswerAsync(Kernel kernel, PromptExecutionSettings promptSettings, ByteString chatHistoryBytes, CancellationToken cancellationToken, ILogger? log = null)
    {
        var completionService = (IChatCompletionService)kernel.Services.GetService(typeof(IChatCompletionService))!;
        ChatHistory chatHistory = JsonSerializer.Deserialize<ChatHistory>(chatHistoryBytes.Span)!;

        var completion = await ExecuteWithThrottleHandlingAsync(async () =>
        {
            string response;
            try
            {
                var result = await completionService.GetChatMessageContentAsync(chatHistory, promptSettings, kernel, cancellationToken);

                log?.PromptHandledResponsePromptResponse(result);

                response = result.ToString();
            }
            catch (Exception ex)
            {
                log?.ErrorHandlingPromptPrompt(ex, chatHistory.Last());

                response = JsonSerializer.Serialize(ex.Message);
            }

            return response;
        }, cancellationToken, log: log).ConfigureAwait(false);
        return JsonSerializer.Serialize(new { completion });
    }
}
