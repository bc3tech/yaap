using Agent.Core.Extensions;

using Google.Protobuf.WellKnownTypes;

using Orchestrator;
using Orchestrator.GrpcServices;

using Yaap.Core.Models;
using Yaap.Server.Abstractions;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddSemanticKernel();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services
    .AddSingleton<IYaapServer<YaapClientDetail, Empty>, Worker>()
    .AddDistributedMemoryCache()
    .AddHttpContextAccessor()
    .AddControllers();

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
