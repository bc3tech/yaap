using A2A.Server.AspNetCore.Services;
using Orchestrator_WS;

using wsAgent.Core.Extensions;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddSemanticKernel();

builder.Services
    .AddSingleton<Orchestrator>()
    .AddHostedService(sp => sp.GetRequiredService<Orchestrator>())
    .AddDistributedMemoryCache()
    .AddLogging(lb =>
    {
        lb.AddSimpleConsole(o =>
        {
            o.SingleLine = true;
            o.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
            o.IncludeScopes = true;
        });
    });

builder.Services.AddHttpContextAccessor();

WebApplication app = ((WebApplicationBuilder)builder).Build();

// Configure the HTTP request pipeline.
app.UseWebSockets();

// Map WebSocket endpoints
app.Map("/ws/orchestrator", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Orchestrator orchestratorService = context.RequestServices.GetRequiredService<Orchestrator>();
        await orchestratorService.HandleWebSocketAsync(webSocket, context.RequestAborted);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

await app.RunAsync();
