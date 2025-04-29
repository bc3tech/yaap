namespace Orchestrator;

using System.Threading;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Grpc.Models;
using Grpc.Net.Client;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.SemanticKernel;

using Yaap.Server.SemanticKernel;

using YaapClientDetail = Yaap.Core.Models.YaapClientDetail;

internal class Worker(Kernel _kernel, ILogger<Worker> _log, IDistributedCache cache, ILoggerFactory loggerFactory) : YaapServer<Empty>(_kernel, cache, loggerFactory)
{
    protected override async Task<string> CallExpertAsync(YaapClientDetail clientDetail, string prompt, CancellationToken cancellationToken)
    {
        var client = new Grpc.Expert.Expert.ExpertClient(GrpcChannel.ForAddress(clientDetail.CallbackUrl!));
        AnswerResponse r = await client.GetAnswerAsync(new AnswerRequest { Prompt = prompt }, cancellationToken: cancellationToken);
        return r.Completion;
    }
}
