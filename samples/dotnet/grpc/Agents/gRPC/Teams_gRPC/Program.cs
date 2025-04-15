using gRPCAgent.Core.Extensions;

using TBAAPI.V3Client.Api;

using Teams_gRPC;

IHostApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddExpert<Agent>()
    .AddSemanticKernel<TeamApi>();

// Add services to the container.
builder.Services.AddGrpc();

WebApplication app = ((WebApplicationBuilder)builder).Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<Agent>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
