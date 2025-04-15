namespace Common.Extensions;
using System;

public static class ProgramHelpers
{
    public static CancellationTokenSource CreateCancellationTokenSource()
    {
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        cts.Token.Register(() => Console.WriteLine("Cancellation requested. Exiting..."));

        return cts;
    }
}
