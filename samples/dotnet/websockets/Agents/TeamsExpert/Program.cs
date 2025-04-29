using Agent.Core.Extensions;

using TBAAPI.V3Client.Api;

using TeamsExpert;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddExpert<Worker>()
    .AddSemanticKernel<TeamApi>();

// Add services to the container.
builder.Services.AddHttpContextAccessor()
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
        Worker agentService = context.RequestServices.GetRequiredService<Worker>();
        await agentService.HandleWebSocketAsync(webSocket, context.RequestAborted);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();
