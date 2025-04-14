using System.Collections.Concurrent;
using System.Diagnostics;

using AgentsMcpServer;
using AgentsMcpServer.GrpcServices;

using Google.Protobuf.WellKnownTypes;

using ModelContextProtocol.Protocol.Messages;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

using Yaap.Server;

ConcurrentDictionary<string, HashSet<IMcpServer>> subscriptions = new();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddSingleton<Server>()
    .AddHostedService(sp => sp.GetRequiredService<Server>())
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
            McpServerTool targetExpert = Server.ConnectedExperts.First(i => i.ProtocolTool.Name == req.Params?.Name);
            return await targetExpert.InvokeAsync(req, ct);
        }
        catch (Exception e)
        {
            Debug.Fail(e.Message);
            return null;
        }
    });

builder.Services.AddSingleton(subscriptions);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services
    .AddSingleton<IYaapServer<Empty>, Server>()
    .AddDistributedMemoryCache()
    .AddHttpContextAccessor()
    .AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddGrpcReflection();
}

WebApplication app = ((WebApplicationBuilder)builder).Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<YaapService>();
app.MapMcp();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

await app.RunAsync();
