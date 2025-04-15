
namespace Common
{
#pragma warning disable CS8019
    using System;

    using Microsoft.Extensions.Logging;
#pragma warning restore CS8019

    static partial class Log
    {

        [LoggerMessage(0, LogLevel.Trace, "*** REQUEST {requestBody}")]
        internal static partial void REQUESTRequestBody(this ILogger logger, string requestBody);

        [LoggerMessage(1, LogLevel.Trace, "*** RESPONSE {responseContent}")]
        internal static partial void RESPONSEResponseContent(this ILogger logger, string responseContent);
    }
}
