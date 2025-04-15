using System.Collections.Concurrent;
using System.Diagnostics;

using AgentsMcpServer;

using ModelContextProtocol.Protocol.Messages;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

using wsAgent.Core.Extensions;

ConcurrentDictionary<string, HashSet<IMcpServer>> subscriptions = new();

var builder = WebApplication.CreateBuilder(args);
builder.AddSemanticKernel();
builder.Services
    .AddHostedService<Server>()
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddMcpServer(o =>
    {
        ((o.Capabilities ??= new()).Tools ??= new()).ListChanged = true;
        o.ServerInfo = new() { Name = "AgentTools", Version = "1.0" };
    })
    .WithListToolsHandler((req, _) =>
    {
        subscriptions.AddOrUpdate(NotificationMethods.ToolListChangedNotification, [req.Server], (_, s) => [.. s, req.Server]);
        return Task.FromResult(new ListToolsResult { Tools = [.. Server.ConnectedExperts.Select(i => i.ProtocolTool)] });
    })
    .WithCallToolHandler(async (req, ct) =>
    {
        try
        {
            var targetExpert = Server.ConnectedExperts.First(i => i.ProtocolTool.Name == req.Params?.Name);
            return await targetExpert.InvokeAsync(req, ct);
        }
        catch (Exception e)
        {
            Debug.Fail(e.Message);
            return null;
        }
    });

builder.Services.AddSingleton(subscriptions);

WebApplication app = builder.Build();

app.MapMcp();
app.UseWebSockets();

app.Map("/ws/register", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Server orchestratorService = context.RequestServices.GetServices<IHostedService>().OfType<Server>().First();
        await orchestratorService.HandleWebSocketAsync(webSocket, context.RequestAborted);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

await app.RunAsync();
