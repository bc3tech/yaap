namespace TeamsExpert;

using Agent.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Yaap.Core.Models;

internal class Worker(IConfiguration appConfig, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, YaapClientDetail clientDetail, Kernel sk, PromptExecutionSettings promptSettings) : Expert(appConfig, loggerFactory, httpClientFactory, clientDetail, sk, promptSettings) { }
