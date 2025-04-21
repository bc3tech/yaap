namespace TBAStatReader_WS;

using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Worker(ILoggerFactory loggerFactory, IConfiguration configuration) : IHostedService
{
    private readonly ILogger _log = loggerFactory.CreateLogger<Worker>();
    private readonly string _OrchestratorEndpoint = Throws.IfNullOrWhiteSpace(configuration[Constants.Configuration.VariableNames.OrchestratorEndpoint]);

    internal static bool WaitingForResponse = false;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Console.WriteLine("Welcome to the TBA Chat bot! What would you like to know about FIRST competitions, past or present?");

        static async Task runSpinnerAsync(CancellationToken ct)
        {
            CircularCharArray progress = CircularCharArray.ProgressSpinner;
            while (!ct.IsCancellationRequested)
            {
                Console.Write(progress.Next());
                Console.CursorLeft = 0;

                await Task.Delay(100, ct);
            }
        }

        var client = new ClientWebSocket();
        // Connect the WebSocket to the endpoint
        await client.ConnectAsync(new Uri(_OrchestratorEndpoint), cancellationToken);

        do
        {
            Console.Write("> ");
            var question = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(question))
            {
                break;
            }

            var timer = Stopwatch.StartNew();

            CancellationTokenSource spinnerCancelToken = new();
            var combinedCancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, spinnerCancelToken.Token);
            var t = Task.Run(() => runSpinnerAsync(combinedCancelToken.Token), combinedCancelToken.Token);

            WaitingForResponse = true;

            // Send the question to the WebSocket server
            var request = new { action = "StreamAnswer", prompt = question };
            var requestJson = JsonSerializer.Serialize(request);
            var requestBytes = Encoding.UTF8.GetBytes(requestJson);
            await client.SendAsync(new ArraySegment<byte>(requestBytes), WebSocketMessageType.Text, true, cancellationToken);

            // Receive and print streaming responses
            var buffer = new byte[4];
            while (client.State is WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (WaitingForResponse && result.Count > 0)
                {
                    WaitingForResponse = false;
                    await spinnerCancelToken.CancelAsync();
                    Console.CursorLeft = 0;
                }

                if (result.MessageType is WebSocketMessageType.Close)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
                    Console.WriteLine("WebSocket connection closed.");
                }
                else if (result.MessageType is WebSocketMessageType.Text && result.Count > 0)
                {
                    var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.Write(response);
                }
                else
                {
                    _log.BinaryMessageReceivedFromWebSocketUnhandled();
                }

                if (result.EndOfMessage)
                {
                    break;
                }
            }

            Console.WriteLine();

            _log.TimeToAnswerTta(timer.Elapsed);
        } while (!cancellationToken.IsCancellationRequested);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

