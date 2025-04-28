namespace wsAgent.Core;
using System;
using System.Collections.Immutable;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using A2A;
using A2A.Client.Services;
using A2A.Models;

using Common;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Task = Task;

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

    public static async Task HandleWebSocketAsync(WebSocket webSocket, Func<WebSocket, WebSocketReceiveResult, string, CancellationToken, Task<string?>> processCallback, CancellationToken cancellationToken, ILogger? log = null)
    {
        WebSocketReceiveResult? result = null;
        try
        {
            while (!cancellationToken.IsCancellationRequested && result?.CloseStatus.HasValue is not true)
            {
                (result, ImmutableArray<byte> socketResponse) = await ReceiveResponseAsync(webSocket, new ArraySegment<byte>(new byte[4 * 1024]), cancellationToken).ConfigureAwait(false);
                var incomingActionRequest = Encoding.UTF8.GetString([.. socketResponse], 0, socketResponse.Length);
                var responseFromActionHandler = await processCallback(webSocket, result, incomingActionRequest, cancellationToken).ConfigureAwait(false);

                if (result.MessageType is not WebSocketMessageType.Close && responseFromActionHandler is not null)
                {
                    await SendMessageAsync(webSocket, responseFromActionHandler, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch (Exception e) when (e is OperationCanceledException or WebSocketException)
        {
            log?.GotExceptionDuringWebsocketReceiveAssumingDirtyClose(e);
        }

        if (result?.CloseStatus is not null)
        {
            try
            {
                await webSocket.CloseOutputAsync(result.CloseStatus.Value, result.CloseStatusDescription, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                log?.HitExceptionWhenTryingToCloseTheWebsocketConnectionDisregarding(e);
            }
        }
    }

    public static Task SendMessageAsync(WebSocket webSocket, string message, CancellationToken cancellationToken) => webSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, cancellationToken);
    public static async Task<string> SendMessageAsync(IA2AProtocolClient a2aclient, string message, CancellationToken cancellationToken)
    {
        var resp = await a2aclient.SendTaskAsync(new A2A.Requests.SendTaskRequest { Params = new() { Message = new A2A.Models.Message { Role = MessageRole.User, Parts = [new A2A.Models.TextPart(message)] } } }, cancellationToken).ConfigureAwait(false);

        return resp.Result.Artifacts.Last().Parts.OfType<TextPart>().Last().Text;
    }

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

    public static async Task<string?> ProcessMessageAsync(WebSocket caller, WebSocketReceiveResult raw, string message, Kernel kernel, PromptExecutionSettings promptSettings, CancellationToken cancellationToken, ILogger? log = null)
    {
        if (raw.MessageType is not WebSocketMessageType.Close)
        {
            JsonElement jsonObject = JsonDocument.Parse(message).RootElement;
            var action = jsonObject.GetProperty("action").GetString();

            switch (action)
            {
                // Add more cases for other actions
                case "GetAnswer":
                    return await GetAnswerAsync(kernel, promptSettings, Throws.IfNullOrWhiteSpace(jsonObject.GetProperty("prompt").GetString()), cancellationToken, log).ConfigureAwait(false);

                //case "StreamAnswer":
                //    await GetAnswerStreamingAsync(caller, kernel, promptSettings, Throws.IfNullOrWhiteSpace(jsonObject.GetProperty("prompt").GetString()), cancellationToken, log).ConfigureAwait(false);
                //    return null;

                default:
                    return JsonSerializer.Serialize(new { error = "Unknown action" });
            }
        }

        return null;
    }

    private static async Task<string> GetAnswerAsync(Kernel kernel, PromptExecutionSettings promptSettings, string prompt, CancellationToken cancellationToken, ILogger? log = null)
    {
        var completion = await ExecuteWithThrottleHandlingAsync(async () =>
        {
            string response;
            try
            {
                FunctionResult promptResult = await kernel.InvokePromptAsync(prompt, new(promptSettings), cancellationToken: cancellationToken).ConfigureAwait(false);

                log?.PromptHandledResponsePromptResponse(promptResult);

                response = promptResult.ToString();
            }
            catch (Exception ex)
            {
                log?.ErrorHandlingPromptPrompt(ex, prompt);

                response = JsonSerializer.Serialize(ex.Message);
            }

            return response;
        }, cancellationToken, log: log).ConfigureAwait(false);
        return JsonSerializer.Serialize(new { completion });
    }
}
