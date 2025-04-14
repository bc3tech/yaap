namespace Teams_SignalR;
using Common.Extensions;

using Microsoft.Extensions.Hosting;

using TBAAPI.V3Client.Api;

internal partial class Program
{
    private static async Task Main(string[] args)
    {
        CancellationTokenSource cts = ProgramHelpers.CreateCancellationTokenSource();

        await Host.CreateApplicationBuilder(args)
            .AddExpert<Agent>()
            .AddSemanticKernel<TeamApi>()
            .Build()
            .RunAsync(cts.Token).ConfigureAwait(false);
    }
}