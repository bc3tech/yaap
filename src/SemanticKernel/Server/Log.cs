
namespace Yaap.Server.SemanticKernel;

#pragma warning disable CS8019
using Microsoft.Extensions.Logging;
using System;
#pragma warning restore CS8019

static partial class Log
{

    [LoggerMessage(0, LogLevel.Debug, "Adding {expertName} to Semantic Kernel plugins...")]
    internal static partial void AddingExpertNameToSemanticKernelPlugins(this ILogger logger, string expertName);

    [LoggerMessage(1, LogLevel.Warning, "Plugin {name} not found, but said Goodbye")]
    internal static partial void PluginNameNotFoundButSaidGoodbye(this ILogger logger, string name);

    [LoggerMessage(2, LogLevel.Debug, "{ClientDetail}")]
    internal static partial void ClientDetail(this ILogger logger, Core.Models.YaapClientDetail ClientDetail);
}
