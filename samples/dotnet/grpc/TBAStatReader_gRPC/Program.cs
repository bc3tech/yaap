using Common;

using ConsoleApp;

using Grpc.Net.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using static Orchestrator_gRPC.Orchestrator;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
        {
            cts.Cancel();
            e.Cancel = true;
        };

cts.Token.Register(() => Console.WriteLine("Cancellation requested. Exiting..."));

HostApplicationBuilder b = Host.CreateApplicationBuilder(args);
b.Services.AddHostedService<Worker>()
    .AddHttpClient()
    .AddTransient<DebugHttpHandler>()
    .AddLogging(lb =>
    {
        lb.AddSimpleConsole(o =>
        {
            o.SingleLine = true;
            o.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
            o.IncludeScopes = true;
        });
    })
    .AddHttpLogging(o => o.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestBody | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseBody);

ILoggerFactory loggerFactory = b.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();

b.Services.AddSingleton(sp => new OrchestratorClient(GrpcChannel.ForAddress(sp.GetRequiredService<IConfiguration>()[Constants.Configuration.VariableNames.OrchestratorEndpoint]!)));

await b.Build().RunAsync(cts.Token);
