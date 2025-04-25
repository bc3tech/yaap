namespace Yaap.A2A.Server.AspNetCore;

using System;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using Yaap.A2A.Core;
using Yaap.A2A.Core.Models;

/// <summary></summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "EA0004:Make types declared in an executable internal", Justification = "No")]
public static class A2AServerExtensions
{
    /// <summary>
    /// Adds the necessary services for the A2AServer to the DI container.
    /// </summary>
    /// <param name="services">The IServiceCollection to configure.</param>
    /// <param name="agentCard">The AgentCard instance to be used.</param>
    public static IServiceCollection AddA2AServer<TTaskManager>(this IServiceCollection services, AgentCard agentCard) where TTaskManager : TaskManager
    {
        ArgumentNullException.ThrowIfNull(agentCard);

        return services
            .AddSingleton(agentCard)
            .AddSingleton<TTaskManager>();
    }

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Configures the routes for the A2AServer on the WebApplication instance.
    /// </summary>
    /// <param name="app">The WebApplication instance to configure.</param>
    /// <param name="endpoint">The endpoint path for the main request handler (default is "/a2a").</param>
    public static void UseA2AServer(this IRouteBuilder app, string endpoint = "/a2a")
    {
        // Main request handler
        app.MapPost(endpoint, async context =>
        {
            TaskManager taskManager = context.RequestServices.GetRequiredService<TaskManager>();
            AgentCard agentCard = context.RequestServices.GetRequiredService<AgentCard>();
            var server = new A2AServer(agentCard, taskManager, endpoint);
            await server.ProcessRequest(context);
        });

        // Agent Card handler
        app.MapGet("/.well-known/agent.json", async context =>
        {
            AgentCard agentCard = context.RequestServices.GetRequiredService<AgentCard>();
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(agentCard, _serializerOptions));
        });
    }
}
