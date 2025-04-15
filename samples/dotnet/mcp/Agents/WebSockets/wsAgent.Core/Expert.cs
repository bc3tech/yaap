namespace wsAgent.Core;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Common;
using Common.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public abstract class Expert : IHostedService
{
    protected readonly IServiceProvider _services;
    protected readonly IConfiguration _config;
    protected readonly ILogger _log;
    protected readonly Kernel _kernel;
    protected readonly PromptExecutionSettings _promptSettings;
    protected readonly IHttpClientFactory _httpFactory;
    private readonly IChatCompletionService _chatService;
    private readonly ClientWebSocket? _webSocket;

    protected Expert(IServiceProvider sp)
    {
        _services = sp;
        _config = sp.GetRequiredService<IConfiguration>();
        _kernel = sp.GetRequiredService<Kernel>();
        _promptSettings = sp.GetRequiredService<PromptExecutionSettings>();
        _httpFactory = sp.GetRequiredService<IHttpClientFactory>();
        _chatService = _kernel.Services.GetRequiredService<IChatCompletionService>();

        this.Name = Throws.IfNullOrWhiteSpace(_config[Constants.Configuration.Paths.AgentName]);
        _log = sp.GetRequiredService<ILoggerFactory>().CreateLogger(this.Name);

        this.Description = _config[Constants.Configuration.Paths.AgentDescription];
        var securePort = _config.GetRequiredSection("Kestrel").GetRequiredSection("Endpoints").GetSection("HTTPs")["Url"];
        if (securePort is not null)
        {
            this.CallbackPort = new Uri(securePort).Port;
            this.Secured = true;
        }
        else
        {
            this.CallbackPort = new Uri(Throws.IfNullOrWhiteSpace(_config.GetRequiredSection("Kestrel").GetRequiredSection("Endpoints").GetRequiredSection("HTTP")["Url"])).Port;
        }

        if (this.PerformsIntroduction)
        {
            _webSocket = new ClientWebSocket();
        }
    }

    public string Name { get; protected init; }
    public string? Description { get; protected init; }
    public int CallbackPort { get; protected init; }
    public bool Secured { get; protected init; }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        AgentDefinition.OutputRegisteredSkFunctions(_kernel, new LogTraceTextWriter(_log));

        if (this.PerformsIntroduction && _webSocket is not null)
        {
            var uri = new Uri(_config[Constants.Configuration.VariableNames.OrchestratorEndpoint]!);
            await _webSocket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
            await IntroduceAsync(cancellationToken).ConfigureAwait(false);
        }

        _log.AwaitingQuestion();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_webSocket?.State is WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken).ConfigureAwait(false);
        }

        _log.SayingGoodbyeToOrchestrator(cancellationToken.IsCancellationRequested);
    }

    protected virtual bool PerformsIntroduction { get; } = true;

    protected async Task IntroduceAsync(CancellationToken cancellationToken)
    {
        _log.IntroducingMyself();
        var message = JsonSerializer.Serialize(new
        {
            action = "Introduce",
            detail = new { this.Name, this.Description, this.CallbackPort, this.Secured }
        });
        await SendMessageAsync(_webSocket!, message, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ChatHistory> GetAnswerAsync(ChatHistory chatHistory, CancellationToken cancellationToken)
    {
        using IDisposable scope = _log.CreateMethodScope();

        _log.GettingAnswerForChatChatHistory(chatHistory);

        return await GetAnswerInternalAsync(chatHistory, cancellationToken).ConfigureAwait(false);
    }

    protected virtual async Task<ChatHistory> GetAnswerInternalAsync(ChatHistory chatHistory, CancellationToken cancellationToken)
    {
        return await ExecuteWithThrottleHandlingAsync(async () =>
        {
            ChatHistory chatHistoryForThisAgent = [];
            try
            {
                chatHistoryForThisAgent = new ChatHistory([new ChatMessageContent(AuthorRole.System, ((OpenAIPromptExecutionSettings)_promptSettings).ChatSystemPrompt), .. chatHistory.Where(i => i.Role != AuthorRole.System)]);

                var completion = await _chatService.GetChatMessageContentAsync(chatHistoryForThisAgent, _promptSettings, _kernel, cancellationToken).ConfigureAwait(false);
                chatHistoryForThisAgent.Add(completion);

                _log.PromptHandledResponsePromptResponse(chatHistoryForThisAgent.Last());
            }
            catch (Exception ex)
            {
                _log.ErrorHandlingPromptPrompt(ex, chatHistoryForThisAgent.Last());

                chatHistoryForThisAgent.AddAssistantMessage(ex.Message);
            }

            return chatHistoryForThisAgent;
        }, cancellationToken).ConfigureAwait(false);
    }

    //private Task StreamAnswerAsync(WebSocket caller, string prompt, CancellationToken cancellationToken)
    //{
    //    return ExecuteWithThrottleHandlingAsync(async () =>
    //    {
    //        string response;
    //        try
    //        {
    //            await foreach (StreamingKernelContent token in _kernel.InvokePromptStreamingAsync(prompt, new(_promptSettings)))
    //            {
    //                await caller.SendAsync(token.ToByteArray(), WebSocketMessageType.Text, false, cancellationToken).ConfigureAwait(false);
    //            }

    //            await caller.SendAsync(Encoding.UTF8.GetBytes(string.Empty), WebSocketMessageType.Text, true, cancellationToken);
    //        }
    //        catch (Exception ex)
    //        {
    //            _log.ErrorHandlingPromptPrompt(ex, prompt);

    //            response = JsonSerializer.Serialize(ex.Message);
    //        }
    //    }, cancellationToken);
    //}

    protected virtual async Task<string> ProcessMessageAsync(WebSocket caller, string message, CancellationToken cancellationToken)
    {
        JsonElement jsonObject = JsonDocument.Parse(message).RootElement;
        var action = jsonObject.GetProperty("action").GetString();

        switch (action)
        {
            case "GetAnswer":
                var chat = Throws.IfNull(jsonObject.GetProperty("prompt").Deserialize<ChatHistory>());
                var completion = await GetAnswerAsync(chat, cancellationToken).ConfigureAwait(false);
                return JsonSerializer.Serialize(new { completion });

            // Add more cases for other actions
            //case "StreamAnswer":
            //    prompt = Throws.IfNullOrWhiteSpace(jsonObject.GetProperty("prompt").GetString());
            //    await StreamAnswerAsync(caller, prompt, cancellationToken).ConfigureAwait(false);
            //    return string.Empty;

            default:
                _log.UnknownActionAction(action);
                return JsonSerializer.Serialize(new { error = "Unknown action" });
        }
    }

    private static Task SendMessageAsync(WebSocket webSocket, string message, CancellationToken cancellationToken) => webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, cancellationToken);

    protected async Task<T> ExecuteWithThrottleHandlingAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken, int maxRetries = 10)
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
                        _log.ResponsesThrottledWaitingRetryAfterSecondsToTryAgain(waitTime);
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

        Debug.Assert(lastException is not null);
        _log.MaxRetriesExceeded(lastException);
        throw lastException!;
    }

    protected async Task ExecuteWithThrottleHandlingAsync(Func<Task> operation, CancellationToken cancellationToken, int maxRetries = 10)
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
                        _log.ResponsesThrottledWaitingRetryAfterSecondsToTryAgain(waitTime);
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

        Debug.Assert(lastException is not null);
        _log.MaxRetriesExceeded(lastException);
        throw lastException!;
    }

    public virtual async Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        WebSocketReceiveResult? result = null;
        try
        {
            do
            {
                (result, ImmutableArray<byte> socketResponse) = await ReceiveSocketResponseAsync(webSocket, cancellationToken).ConfigureAwait(false);
                var incomingActionRequest = Encoding.UTF8.GetString([.. socketResponse], 0, socketResponse.Length);
                var responseFromActionHandler = await ProcessMessageAsync(webSocket, incomingActionRequest, cancellationToken).ConfigureAwait(false);

                await SendMessageAsync(webSocket, responseFromActionHandler, cancellationToken).ConfigureAwait(false);
            } while (!cancellationToken.IsCancellationRequested && !result.CloseStatus.HasValue);
        }
        catch (Exception e) when (e is OperationCanceledException or WebSocketException)
        {
            _log.GotExceptionDuringWebsocketReceiveAssumingDirtyClose(e);
        }

        if (result?.CloseStatus is not null)
        {
            try
            {
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _log.HitExceptionWhenTryingToCloseTheWebsocketConnectionDisregarding(e);
            }
        }
    }

    private static async Task<(WebSocketReceiveResult lastReceiveResult, ImmutableArray<byte> responseBytes)> ReceiveSocketResponseAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        List<byte> webSocketResponseBytes = [];
        var buffer = new ArraySegment<byte>(new byte[1024 * 4]);

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
}
