namespace Teams_gRPC;

using Grpc.YaapServer;

using gRPCAgent.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

internal class Agent(IConfiguration appConfig, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, Kernel sk, PromptExecutionSettings promptSettings, YaapServer.YaapServerClient yaapServer) : Expert(appConfig, loggerFactory, httpClientFactory, sk, promptSettings, yaapServer) { }
