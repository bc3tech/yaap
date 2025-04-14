namespace AgentsMcpServer.GrpcServices;

using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;
using Grpc.YaapServer;

internal class YaapService(Server yaapServer) : YaapServer.YaapServerBase
{
    public override Task<Empty> Hello(YaapClientDetail request, ServerCallContext context) => yaapServer.HandleHelloAsync(request, context.CancellationToken);

    public override async Task<Empty> Goodbye(YaapClientDetail request, ServerCallContext context)
    {
        await yaapServer.HandleGoodbyeAsync(request, context.CancellationToken).ConfigureAwait(false);
        return new();
    }
}
