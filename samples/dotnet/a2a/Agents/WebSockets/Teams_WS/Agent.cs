namespace Teams_WS;

using A2A.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using wsAgent.Core;

internal class Agent(IConfiguration appConfig, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, AgentCard card, Kernel sk, PromptExecutionSettings promptSettings) : Expert(appConfig, loggerFactory, httpClientFactory, card, sk, promptSettings) { }
