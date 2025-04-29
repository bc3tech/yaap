namespace TeamsExpert;

using Agent.Core;

using Grpc.YaapServer;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

internal class Worker(IConfiguration appConfig, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, Kernel sk, PromptExecutionSettings promptSettings, YaapServer.YaapServerClient yaapServer) : Expert(appConfig, loggerFactory, httpClientFactory, sk, promptSettings, yaapServer) { }
