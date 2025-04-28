namespace ConsoleApp;

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Common;

using Google.Protobuf;

using Grpc.Core;
using Grpc.Models;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;

using static Grpc.Orchestrator.Orchestrator;

internal class Worker(ILoggerFactory loggerFactory, OrchestratorClient client) : IHostedService
{
    private readonly ILogger _log = loggerFactory.CreateLogger<Worker>();

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

            var newChat = new ChatHistory();
            newChat.AddUserMessage(question);

            AsyncServerStreamingCall<StreamResponse> completionCall = client.GetAnswerStream(new AnswerRequest { ChatHistory = ByteString.CopyFrom(JsonSerializer.SerializeToUtf8Bytes(newChat)) }, cancellationToken: cancellationToken);
            await foreach (StreamResponse? r in completionCall.ResponseStream.ReadAllAsync(cancellationToken: cancellationToken))
            {
                if (WaitingForResponse && r.CalculateSize() is not 0 && !string.IsNullOrEmpty(r.Token))
                {
                    WaitingForResponse = false;
                    await spinnerCancelToken.CancelAsync();
                    Console.CursorLeft = 0;
                }

                Console.Write(r.Token);
            }

            Console.WriteLine();

            _log.TimeToAnswerTta(timer.Elapsed);
        } while (!cancellationToken.IsCancellationRequested);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}