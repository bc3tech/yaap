namespace Yaap.Common;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

internal static class Throws
{
    [return: NotNull]
    public static string IfNullOrWhiteSpace([NotNull] string? value, string? message = null, [CallerArgumentExpression(nameof(value))] string? variableName = null) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException(variableName, message) : value;

    [return: NotNull]
    public static string IfNullOrEmpty([NotNull] string? value, string? message = null, [CallerArgumentExpression(nameof(value))] string? variableName = null) => string.IsNullOrEmpty(value) ? throw new ArgumentNullException(variableName, message) : value;

    [return: NotNull]
    public static T IfNull<T>([NotNull] T? value, string? message = null, [CallerArgumentExpression(nameof(value))] string? variableName = null) => value ?? throw new ArgumentNullException(variableName, message);
}
