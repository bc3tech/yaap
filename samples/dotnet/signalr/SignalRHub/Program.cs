using Common;

using Microsoft.AspNetCore.SignalR;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services
    .AddSignalR(o => o.EnableDetailedErrors = true)
    .AddAzureSignalR(o => o.ConnectionString = builder.Configuration["Azure:SignalR:ConnectionString"]!);

builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.MapHub<SignalRHub.SignalRHub>("/api");

await app.RunAsync();
