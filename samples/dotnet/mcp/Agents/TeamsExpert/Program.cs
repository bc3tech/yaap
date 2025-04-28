using Agent.Core.Extensions;

using Grpc.YaapServer;

using TBAAPI.V3Client.Api;

using TeamsExpert;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddExpert<Worker>()
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
    builder.Services.AddGrpcClient<YaapServer.YaapServerClient>(o => o.Address = new Uri(builder.Configuration["Yaap:Server:Endpoint"]!));
}

WebApplication app = ((WebApplicationBuilder)builder).Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<Worker>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();
