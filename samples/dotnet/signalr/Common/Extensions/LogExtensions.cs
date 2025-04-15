namespace Common.Extensions;
using System;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;

public static class LogExtensions
{
    public static IDisposable CreateMethodScope(this ILogger logger, [CallerMemberName] string? methodName = null) => logger.BeginScope(Throws.IfNullOrWhiteSpace(methodName))!;
}
