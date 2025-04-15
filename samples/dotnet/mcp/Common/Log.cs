
namespace Common
{
    using Microsoft.Extensions.Logging;

    static partial class Log
    {

        [LoggerMessage(0, LogLevel.Trace, "*** REQUEST {requestBody}")]
        internal static partial void REQUESTRequestBody(this ILogger logger, string requestBody);

        [LoggerMessage(1, LogLevel.Trace, "*** RESPONSE {responseContent}")]
        internal static partial void RESPONSEResponseContent(this ILogger logger, string responseContent);
    }
}
