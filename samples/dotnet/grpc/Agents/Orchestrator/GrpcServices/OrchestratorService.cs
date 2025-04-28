namespace Orchestrator.GrpcServices;

using Agent.Core;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;
using Grpc.Models;
using Grpc.Orchestrator;

using Microsoft.SemanticKernel;

using Yaap.Server.Abstractions;

internal class OrchestratorService(IYaapServer<Yaap.Core.Models.YaapClientDetail, Empty> orchestrator, ILogger<OrchestratorService> _log, Kernel _kernel, PromptExecutionSettings _promptSettings) : Orchestrator.OrchestratorBase
{
    public override Task<Empty> Hello(YaapClientDetail request, ServerCallContext context) => orchestrator.HandleHelloAsync(request, context.CancellationToken);

    public override Task<Empty> Message(MessageRequest request, ServerCallContext context)
    {
        _log.GRPCMessageReceivedGrpcMessage(request.Message);

        return Task.FromResult(new Empty());
    }

    public override async Task<AnswerResponse> GetAnswer(AnswerRequest request, ServerCallContext context)
    {
        var answer = await AIHelpers.GetAnswerAsync(_kernel, _promptSettings, request.Prompt, context.CancellationToken, _log);

        return new() { Completion = answer };
    }

    public override Task GetAnswerStream(AnswerRequest request, IServerStreamWriter<StreamResponse> responseStream, ServerCallContext context) => AIHelpers.GetAnswerStreamingAsync(_kernel, _promptSettings, request.Prompt, responseStream, context.CancellationToken, _log);

    public override async Task<Empty> Goodbye(YaapClientDetail request, ServerCallContext context)
    {
        await orchestrator.HandleGoodbyeAsync(request, context.CancellationToken);

        return new();
    }
}
