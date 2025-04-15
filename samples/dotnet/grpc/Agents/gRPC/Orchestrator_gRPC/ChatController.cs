namespace Orchestrator_gRPC;

using System.Text.Json.Nodes;

using Common;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ChatController(OrchestratorExpert orchestrator) : ControllerBase
{
    [HttpPost("GetCompletion")]
    public async Task<IActionResult> GetCompletionAsync(CancellationToken cancellationToken)
    {
        var req = this.HttpContext.Request;
        var body = await req.ReadFromJsonAsync<JsonObject>();
        var prompt = Throws.IfNullOrWhiteSpace(body?["prompt"]?.ToString());
        var r = await orchestrator.GetAnswer(prompt, cancellationToken);
        return Ok(r.Completion);
    }
}
