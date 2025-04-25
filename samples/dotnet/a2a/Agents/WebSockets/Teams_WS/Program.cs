
using Common;

using TBAAPI.V3Client.Api;

using Teams_WS;

using wsAgent.Core.Extensions;

using Yaap.A2A.Core;
using Yaap.A2A.Core.Models;
using Yaap.A2A.Server.AspNetCore;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddExpert<Agent>()
    .AddSemanticKernel<TeamApi>();

// Add services to the container.
builder.Services
    .AddA2AServer<InMemoryTaskManager>(new AgentCard
    (
        Name: builder.Configuration[Constants.Configuration.Paths.AgentName],
        Description: builder.Configuration[Constants.Configuration.VariableNames.Description],
        Url: new Uri(builder.Configuration["Yaap:Client:CallbackUrl"]!),
   ))
    .AddHttpContextAccessor()
    .AddControllers();

WebApplication app = ((WebApplicationBuilder)builder).Build();

// Configure the HTTP request pipeline.
app.UseWebSockets();
app.MapGet("/", () => "Communication with WebSocket endpoints must be made through a WebSocket client.");

// Map WebSocket endpoints
app.Map("/ws/agent", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Agent agentService = context.RequestServices.GetRequiredService<Agent>();
        await agentService.HandleWebSocketAsync(webSocket, context.RequestAborted);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();
