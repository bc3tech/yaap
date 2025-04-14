using Common;

using gRPCAgent.Core.Extensions;

using ModelContextProtocol.Protocol.Transport;

using Orchestrator_gRPC;
using Orchestrator_gRPC.GrpcServices;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddExpert<Orchestrator>()
    .AddSemanticKernel();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services
    .AddDistributedMemoryCache()
    .AddHttpContextAccessor()
    .AddControllers();

builder.Services.AddSingleton<IClientTransport>(sp => new SseClientTransport(new() { Endpoint = new(builder.Configuration[Constants.Configuration.VariableNames.McpServerEndpoint]!) }, sp.GetRequiredService<IHttpClientFactory>().CreateClient("mcpClient"), sp.GetService<ILoggerFactory>()));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddGrpcReflection();
}

WebApplication app = ((WebApplicationBuilder)builder).Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<OrchestratorService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();
