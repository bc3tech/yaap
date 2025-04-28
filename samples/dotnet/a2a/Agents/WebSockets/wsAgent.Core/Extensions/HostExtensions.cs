namespace wsAgent.Core.Extensions;

using A2A.Server;
using A2A.Server.Infrastructure;
using A2A.Server.Infrastructure.Services;

using Assistants;

using Azure.Identity;

using Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using TBAAPI.V3Client.Client;

public static class HostExtensions
{
    public static IHostApplicationBuilder AddExpert<T>(this IHostApplicationBuilder b, Uri a2aEndpoint) where T : notnull, Expert => b.AddExpert<T, IHostApplicationBuilder>(a2aEndpoint);
    public static R AddExpert<T, R>(this R b, Uri a2aEndpoint) where T : notnull, Expert where R : IHostApplicationBuilder
    {
        ValidateConfigForExpert(b.Configuration);
        if (a2aEndpoint.IsAbsoluteUri)
        {
            throw new ArgumentException("A2A endpoints must be relative");
        }

        b.Services.AddSingleton<T>()
            .AddSingleton<IAgentRuntime>(sp => sp.GetRequiredService<T>())
            .AddHostedService(sp => sp.GetRequiredService<T>())
            .AddHttpClient()
            .AddTransient<DebugHttpHandler>()
            .AddLogging(lb =>
            {
                lb.AddSimpleConsole(o =>
                {
                    o.SingleLine = true;
                    o.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                    o.IncludeScopes = true;
                });
            })
            .AddA2AProtocolServer(b.Configuration[Constants.Configuration.Paths.AgentName]!, server =>
            {
                server.UseAgentRuntime(p => p.GetRequiredService<IAgentRuntime>())
                    .WithLifetime(ServiceLifetime.Singleton)
                    .SupportsStreaming()
                    .UseDistributedCacheTaskRepository();
            })
            .AddA2AWellKnownAgent((sp, cardBuilder) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                cardBuilder
                    .WithName(config[Constants.Configuration.Paths.AgentName]!)
                    .WithDescription(config[Constants.Configuration.Paths.AgentDescription]!)
                    .WithVersion("0.1.0")
                    .WithUrl(new Uri(config["Yaap:Client:CallbackUrl"]!))
                    .WithProvider(pb =>
                    {
                        pb.WithOrganization("BC3 Technologies")
                            .WithUrl(new("http://yaap.bc3.tech"));
                    })
                    .SupportsStreaming();
            });

        return b;
    }

    private static void ValidateConfigForExpert(IConfigurationManager configuration)
    {
        var agentName = configuration[Constants.Configuration.Paths.AgentName];
        Throws.IfNullOrWhiteSpace(agentName);
        Throws.IfNullOrWhiteSpace(configuration[Constants.Configuration.VariableNames.OrchestratorEndpoint]);
    }

    public static IHostApplicationBuilder AddSemanticKernel(this IHostApplicationBuilder b, Action<IServiceProvider, OpenAIPromptExecutionSettings>? configurePromptSettings = default, Action<IServiceProvider, IKernelBuilder>? configureKernelBuilder = default, Action<IServiceProvider, Kernel>? configureKernel = default)
    {
        ValidateConfigForSemanticKernel(b.Configuration);

        b.Services
            .AddHttpClient()
            .AddLogging()
            .AddSingleton<PromptExecutionSettings>(sp =>
            {
                var settings = new OpenAIPromptExecutionSettings
                {
                    ChatSystemPrompt = Throws.IfNullOrWhiteSpace(b.Configuration[Constants.Configuration.VariableNames.SystemPrompt], message: "Missing SystemPrompt environment variable"),
                    Temperature = 0.1,
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                    User = Environment.MachineName
                };

                configurePromptSettings?.Invoke(sp, settings);

                return settings;
            })
            .AddSingleton(sp =>
            {
                IHttpClientFactory httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                ILogger logger = loggerFactory.CreateLogger("KernelDI");

                IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
                kernelBuilder.Services.AddSingleton(loggerFactory);
                kernelBuilder.Plugins.AddFromType<Calendar>();

                var endpoint = b.Configuration[Constants.Configuration.VariableNames.AzureOpenAIEndpoint];
                logger.AzureOpenAIEndpointAzureOpenAIEndpoint(endpoint);
                if (endpoint is not null)
                {
                    if (b.Configuration["AzureOpenAIKey"] is not null)
                    {
                        logger.UsingAzureOpenAIKey();
                        kernelBuilder.AddAzureOpenAIChatCompletion(
                            b.Configuration[Constants.Configuration.VariableNames.AzureOpenAIModelDeployment]!,
                            endpoint,
                            b.Configuration["AzureOpenAIKey"]!,
                            httpClient: httpClientFactory.CreateClient("AzureOpenAi"));
                    }
                    else
                    {
                        logger.UsingIdentity();
                        kernelBuilder.AddAzureOpenAIChatCompletion(
                            b.Configuration[Constants.Configuration.VariableNames.AzureOpenAIModelDeployment]!,
                            endpoint,
                            new DefaultAzureCredential(),
                            httpClient: httpClientFactory.CreateClient("AzureOpenAi"));
                    }
                }

                endpoint = b.Configuration["OpenAIEndpoint"];
                logger.OpenAIEndpointOpenAIEndpoint(endpoint);
                if (endpoint is not null)
                {
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                    kernelBuilder.AddOpenAIChatCompletion(b.Configuration["OpenAIModelId"]!, new Uri(endpoint), b.Configuration["OpenAIKey"] ?? string.Empty);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                }

                configureKernelBuilder?.Invoke(sp, kernelBuilder);

                Kernel kernel = kernelBuilder.Build();
                configureKernel?.Invoke(sp, kernel);

                return kernel;
            });

        return b;
    }

    private static void ValidateConfigForSemanticKernel(IConfigurationManager config)
    {
        var azureOpenAiEndpointValue = config[Constants.Configuration.VariableNames.AzureOpenAIEndpoint];

        if (!string.IsNullOrEmpty(azureOpenAiEndpointValue))
        {
            Throws.IfNullOrWhiteSpace(config[Constants.Configuration.VariableNames.AzureOpenAIModelDeployment]);
        }

        var openaiEndpoint = config["OpenAIEndpoint"];
        if (!string.IsNullOrEmpty(openaiEndpoint))
        {
            if (!string.IsNullOrEmpty(azureOpenAiEndpointValue))
            {
                throw new ArgumentException("Only one of 'AzureOpenAIEndpoint' or 'OpenAIEndpoint' can be specified. Check your configuration and try again.");
            }

            Throws.IfNullOrWhiteSpace(config["OpenAIModelId"]);

            if (string.IsNullOrWhiteSpace(config["OpenAIKey"]))
            {
                if (!openaiEndpoint.Contains("localhost", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Missing 'OpenAIKey'");
                }
            }
        }
    }

    public static IHostApplicationBuilder AddSemanticKernel<TApi>(this IHostApplicationBuilder b, Action<IServiceProvider, OpenAIPromptExecutionSettings>? configurePromptSettings = default, Action<IServiceProvider, IKernelBuilder>? configureKernel = default) => b.AddSemanticKernel(configurePromptSettings,
        (sp, kb) =>
        {
            var expert = (TApi)Activator.CreateInstance(typeof(TApi), new Configuration(new Dictionary<string, string>(),
                new Dictionary<string, string>() { { "X-TBA-Auth-Key", Throws.IfNullOrWhiteSpace(b.Configuration[Constants.Configuration.VariableNames.TBA_API_KEY], message: "Missing TBA_API_KEY environment variable") } },
                new Dictionary<string, string>()), sp.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(TApi).Name))!;
            kb.Plugins.AddFromObject(expert);

            configureKernel?.Invoke(sp, kb);
        });
}
