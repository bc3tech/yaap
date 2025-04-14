namespace Orchestrator_SignalR;

using Common.Extensions;

using Microsoft.Extensions.Hosting;

internal partial class Program
{
    private static async Task Main(string[] args)
    {
        CancellationTokenSource cts = ProgramHelpers.CreateCancellationTokenSource();

        HostApplicationBuilder b = Host.CreateApplicationBuilder(args)
            .AddExpert<Agent>()
            .AddSemanticKernel();

        await b.Build().RunAsync(cts.Token);
    }
}