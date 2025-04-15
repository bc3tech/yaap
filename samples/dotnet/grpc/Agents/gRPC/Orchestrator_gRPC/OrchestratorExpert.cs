namespace Orchestrator_gRPC;

using System.Collections.Concurrent;

using Agent_gRPC;

using Grpc.Net.Client;

using gRPCAgent.Core;

using Microsoft.SemanticKernel;

public class OrchestratorExpert(IConfiguration configuration, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, Kernel kernel, PromptExecutionSettings promptSettings) : Expert(configuration, loggerFactory, httpClientFactory, kernel, promptSettings)
{
    private readonly static ConcurrentDictionary<string, Agent.AgentClient> _experts = new();

    private readonly ILogger _log = loggerFactory.CreateLogger<OrchestratorExpert>();

    protected override bool PerformsIntroduction { get; } = false;

    internal void AddAgent(AgentDetail request, Grpc.Core.ServerCallContext context)
    {
        _log.AddingExpertNameToPanel(request.Name);
        _log.LogDebug("{0}", request);

        _experts.AddOrUpdate(request.Name, (_, addr) => new Agent.AgentClient(addr), (_, _, addr) => new Agent.AgentClient(addr), GrpcChannel.ForAddress(request.CallbackAddress));

        _kernel.ImportPluginFromFunctions(request.Name, [_kernel.CreateFunctionFromMethod(async (string prompt) => {
            var r = await _experts[request.Name].GetAnswerAsync(new Expert_gRPC.AnswerRequest{ Prompt=prompt });
            return r.Completion;
        },
            request.Name, request.Description,
            [new ("prompt") { IsRequired = true, ParameterType = typeof(string) }],
            new () { Description = "Prompt response as a JSON object or array to be inferred upon.", ParameterType = typeof(string) })]
        );
    }

}
