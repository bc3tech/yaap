namespace Teams_gRPC;

using Grpc.Orchestrator;

using gRPCAgent.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

internal class Agent(IConfiguration appConfig, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, Kernel sk, PromptExecutionSettings promptSettings, Orchestrator.OrchestratorClient orchestrator) : Expert(appConfig, loggerFactory, httpClientFactory, sk, promptSettings, orchestrator) { }
