namespace Orchestrator.GrpcServices;

using Agent.Core;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;
using Grpc.Models;
using Grpc.Orchestrator;

using Microsoft.SemanticKernel;

internal class OrchestratorService(ILogger<OrchestratorService> _log, Kernel _kernel, PromptExecutionSettings _promptSettings) : Orchestrator.OrchestratorBase
{
    public override Task<Empty> Message(MessageRequest request, ServerCallContext context)
    {
        _log.GRPCMessageReceivedGrpcMessage(request.Message);

        return Task.FromResult(new Empty());
    }

    public override async Task<AnswerResponse> GetAnswer(AnswerRequest request, ServerCallContext context)
    {
        var answer = await AIHelpers.GetAnswerAsync(_kernel, _promptSettings, request.ChatHistory, context.CancellationToken, _log);

        return new() { Completion = answer };
    }

    public override Task GetAnswerStream(AnswerRequest request, IServerStreamWriter<StreamResponse> responseStream, ServerCallContext context) => AIHelpers.GetAnswerStreamingAsync(_kernel, _promptSettings, request.ChatHistory, responseStream, context.CancellationToken, _log);
}
