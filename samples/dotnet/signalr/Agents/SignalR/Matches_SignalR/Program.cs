namespace Matches_SignalR;

using Common.Extensions;

using Microsoft.Extensions.Hosting;

using TBAAPI.V3Client.Api;

internal partial class Program
{
    private static async Task Main(string[] args)
    {
        CancellationTokenSource cts = ProgramHelpers.CreateCancellationTokenSource();

        HostApplicationBuilder b = Host.CreateApplicationBuilder(args);
        b.AddExpert<Agent>();

        b.AddSemanticKernel<MatchApi>();

        await b.Build().RunAsync(cts.Token).ConfigureAwait(false);
    }
}