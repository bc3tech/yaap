namespace Teams_SignalR;

using Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

internal class Agent(IConfiguration appConfig, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, Kernel sk, PromptExecutionSettings promptSettings) : Expert(appConfig, loggerFactory, httpClientFactory, sk, promptSettings) { }
