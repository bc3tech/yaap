namespace Yaap.Server.SemanticKernel;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Yaap.Core.Models;
using Yaap.Server.Abstractions;

/// <summary>
/// Represents a server that integrates with the Semantic Kernel framework.
/// </summary>
/// <typeparam name="THelloResponse">The type of the response returned by the Hello operation.</typeparam>
public abstract class YaapServer<THelloResponse>(Kernel kernel, IDistributedCache cache, ILoggerFactory loggerFactory)
    : BaseYaapServer<THelloResponse>(cache, loggerFactory) where THelloResponse : notnull, new()
{
    private readonly ILogger _log = loggerFactory.CreateLogger("Yaap.Server.SemanticKernel");

    /// <summary>
    /// Calls an expert asynchronously with the provided prompt.
    /// </summary>
    /// <param name="clientDetail">Details of the client making the request.</param>
    /// <param name="prompt">The prompt to be processed by the expert.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the expert's response.</returns>
    protected abstract Task<string> CallExpertAsync(YaapClientDetail clientDetail, string prompt, CancellationToken cancellationToken);

    /// <inheritdoc />
    protected override Task<THelloResponse> HandleHelloCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
    {
        _log.AddingExpertNameToSemanticKernelPlugins(clientDetail.Name);
        _log.ClientDetail(clientDetail);

        kernel.ImportPluginFromFunctions(clientDetail.Name, [kernel.CreateFunctionFromMethod(
            async (string prompt) => await CallExpertAsync(clientDetail, prompt, cancellationToken),
            clientDetail.Name, clientDetail.Description,
            [new ("prompt") { IsRequired = true, ParameterType = typeof(string) }],
            new () { Description = "Prompt response as a JSON object or array to be inferred upon.", ParameterType = typeof(string) })]
        );

        return Task.FromResult(new THelloResponse());
    }

    /// <inheritdoc />
    protected override Task HandleGoodbyeCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
    {
        if (!kernel.Plugins.TryGetPlugin(clientDetail.Name, out KernelPlugin? plugin) || plugin is null)
        {
            _log.PluginNameNotFoundButSaidGoodbye(clientDetail.Name);
            Debug.Fail(null);
        }
        else
        {
            kernel.Plugins.Remove(plugin);
        }

        return Task.CompletedTask;
    }
}
