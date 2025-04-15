using Common;

using ModelContextProtocol.Protocol.Transport;

using Orchestrator_WS;

using wsAgent.Core.Extensions;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddExpert<Orchestrator>()
    .AddSemanticKernel();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IClientTransport>(sp => new SseClientTransport(new() { Endpoint = new(builder.Configuration[Constants.Configuration.VariableNames.OrchestratorEndpoint]!) }, sp.GetRequiredService<IHttpClientFactory>().CreateClient("mcpClient"), sp.GetService<ILoggerFactory>()));

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

app.Run();
