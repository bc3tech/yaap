namespace Orchestrator_gRPC.Services;

using Common.Extensions;

using Expert_gRPC;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Orchestrator_gRPC;

internal class OrchestratorService(OrchestratorExpert _expert, ILogger<OrchestratorService> _log) : Orchestrator.OrchestratorBase
{
    public override Task<IntroResponse> Introduce(AgentDetail request, ServerCallContext context)
    {
        using IDisposable scope = _log.CreateMethodScope();

        _expert.AddAgent(request, context);

        _log.ExpertExpertNameAdded(request.Name);
        return Task.FromResult(new IntroResponse() { Success = true });
    }

    public override Task<Empty> Message(MessageRequest request, ServerCallContext context)
    {
        _log.LogInformation(request.Message);
        return Task.FromResult(new Empty());
    }

    public override Task<AnswerResponse> GetAnswer(AnswerRequest request, ServerCallContext context) => _expert.GetAnswer(request.Prompt, context.CancellationToken);

    public override Task GetAnswerStream(AnswerRequest request, IServerStreamWriter<StreamResponse> responseStream, ServerCallContext context) => _expert.GetAnswerStream(request, responseStream, context);
}
