namespace TeamsExpert;

using Agent.Core;

using Grpc.Orchestrator;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

internal class Worker(IConfiguration appConfig, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, Kernel sk, PromptExecutionSettings promptSettings, Orchestrator.OrchestratorClient orchestrator) : Expert(appConfig, loggerFactory, httpClientFactory, sk, promptSettings, orchestrator) { }
