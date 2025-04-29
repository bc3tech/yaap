using Agent.Core.Extensions;

using Orchestrator;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddSemanticKernel();

builder.Services
    .AddSingleton<Worker>()
    .AddHostedService(sp => sp.GetRequiredService<Worker>())
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
        Worker orchestratorService = context.RequestServices.GetRequiredService<Worker>();
        await orchestratorService.HandleWebSocketAsync(webSocket, context.RequestAborted);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

await app.RunAsync();
