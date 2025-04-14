using Grpc.Orchestrator;

using gRPCAgent.Core.Extensions;

using TBAAPI.V3Client.Api;

using Teams_gRPC;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddExpert<Agent>()
    .AddSemanticKernel<TeamApi>();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// Add services to the container.
builder.Services.AddGrpc();
if (!string.IsNullOrWhiteSpace(builder.Configuration["Yaap:Server:Endpoint"]))
{
    builder.Services.AddGrpcClient<Orchestrator.OrchestratorClient>(o => o.Address = new Uri(builder.Configuration["Yaap:Server:Endpoint"]!));
}

WebApplication app = ((WebApplicationBuilder)builder).Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<Agent>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();
