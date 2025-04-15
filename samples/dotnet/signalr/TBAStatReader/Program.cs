using System.Net.Http.Json;

using Common;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TBAStatReader;

internal partial class Program
{
    private static async Task Main(string[] args)
    {
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

        var client = new HttpClient();
        HttpResponseMessage hubNegotiateResponse = new();
        ILogger negotiationLogger = loggerFactory.CreateLogger("negotiation");
        for (var i = 0; i < 10; i++)
        {
            try
            {
                hubNegotiateResponse = await client.PostAsync($@"{b.Configuration[Constants.Configuration.VariableNames.SignalREndpoint]}?userid={Constants.SignalR.Users.EndUser}", null, cts.Token);
                break;
            }
            catch (Exception e)
            {
                negotiationLogger.LogDebug(e, $@"Negotiation failed");
                await Task.Delay(1000);
            }
        }

        if (hubNegotiateResponse is null)
        {
            negotiationLogger.LogCritical("Unable to connect to server. Exiting.");
            return;
        }

        hubNegotiateResponse.EnsureSuccessStatusCode();

        Models.SignalR.ConnectionInfo? connInfo;
        try
        {
            connInfo = await hubNegotiateResponse.Content.ReadFromJsonAsync<Models.SignalR.ConnectionInfo>();
        }
        catch (Exception ex)
        {
            negotiationLogger.LogDebug(ex, "Error parsing negotiation response");
            negotiationLogger.LogCritical("Unable to connect to server. Exiting.");
            return;
        }

        ArgumentNullException.ThrowIfNull(connInfo);

        HubConnection hubConn = new HubConnectionBuilder()
            .WithUrl(connInfo.Url, o => o.AccessTokenProvider = connInfo.GetAccessToken)
            .ConfigureLogging(lb => lb
                .AddConfiguration(b.Configuration.GetSection("Logging"))
                .AddSimpleConsole(o =>
                {
                    o.SingleLine = true;
                    o.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                    o.IncludeScopes = true;
                })
            ).WithAutomaticReconnect()
            .WithStatefulReconnect()
            .Build();

        b.Services.AddSingleton(hubConn);

        await b.Build().RunAsync(cts.Token);
    }
}
