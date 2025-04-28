namespace TeamsExpert;

using A2A.Models;

using Agent.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

internal class Worker(IConfiguration appConfig, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, AgentCard card, Kernel sk, PromptExecutionSettings promptSettings) : Expert(appConfig, loggerFactory, httpClientFactory, card, sk, promptSettings) { }
