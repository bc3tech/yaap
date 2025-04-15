namespace Common;

using System.Diagnostics.CodeAnalysis;
using System.Text;

using Microsoft.Extensions.Logging;

[SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Helper methods")]
public class LogTraceTextWriter(ILogger log) : TextWriter
{
    public override Encoding Encoding => Encoding.Default;

    public override void Write(string? value) => log.LogTrace(value);

    public override void Write([StringSyntax("CompositeFormat")] string format, params object?[] arg) => log.LogTrace(format, arg);
}
