using Common;

using ConsoleApp;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
    });

ILoggerFactory loggerFactory = b.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();

await b.Build().RunAsync(cts.Token);
