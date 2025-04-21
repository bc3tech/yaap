namespace ConsoleApp;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Common;

using Grpc.Core;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Worker(ILoggerFactory loggerFactory, Orchestrator_gRPC.Orchestrator.OrchestratorClient client) : IHostedService
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
            AsyncServerStreamingCall<Expert_gRPC.StreamResponse> completionCall = client.GetAnswerStream(new Expert_gRPC.AnswerRequest { Prompt = question }, cancellationToken: cancellationToken);
            await foreach (var r in completionCall.ResponseStream.ReadAllAsync())
            {
                if (WaitingForResponse)
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