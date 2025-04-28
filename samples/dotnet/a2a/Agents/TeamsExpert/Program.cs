using A2A.Server.AspNetCore;

using TBAAPI.V3Client.Api;

using Teams_WS;

using wsAgent.Core.Extensions;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddExpert<Agent>(new("/a2a/TeamsExpert", UriKind.Relative))
    .AddSemanticKernel<TeamApi>();

// Add services to the container.
builder.Services
    .AddHttpContextAccessor()
    .AddControllers();

WebApplication app = ((WebApplicationBuilder)builder).Build();

// Configure the HTTP request pipeline.
//app.UseWebSockets();
app.MapGet("/", () => "Communication with WebSocket endpoints must be made through a WebSocket client.");

// Map WebSocket endpoints
app.MapA2AWellKnownAgentEndpoint();
app.MapA2AAgentHttpEndpoint();
//app.MapA2AAgentWebSocketEndpoint();
//app.Map("/ws/agent", async context =>
//{
//    if (context.WebSockets.IsWebSocketRequest)
//    {
//        System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
//        Agent agentService = context.RequestServices.GetRequiredService<Agent>();
//        await agentService.HandleWebSocketAsync(webSocket, context.RequestAborted);
//    }
//    else
//    {
//        context.Response.StatusCode = 400;
//    }
//});

await app.RunAsync();
