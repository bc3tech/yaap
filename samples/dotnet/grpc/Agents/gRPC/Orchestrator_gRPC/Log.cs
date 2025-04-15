
namespace Orchestrator_gRPC
{
#pragma warning disable CS8019
    using System;

    using Microsoft.Extensions.Logging;
#pragma warning restore CS8019

    static partial class Log
    {

        [LoggerMessage(0, LogLevel.Debug, "Adding {expertName} to panel...")]
        internal static partial void AddingExpertNameToPanel(this ILogger logger, string expertName);

        [LoggerMessage(1, LogLevel.Trace, "Expert {expertName} added.")]
        internal static partial void ExpertExpertNameAdded(this ILogger logger, string expertName);
    }
}
