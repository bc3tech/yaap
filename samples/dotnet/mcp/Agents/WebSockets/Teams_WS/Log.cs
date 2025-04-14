namespace TBAAPI.V3Client.Api;

using System.Text.Json;

using Microsoft.Extensions.Logging;

static partial class Log
{

    [LoggerMessage(0, LogLevel.Debug, "Resulting document: {searchResults}")]
    internal static partial void ResultingDocumentSearchResults(this ILogger logger, string searchResults);

    [LoggerMessage(2, LogLevel.Trace, "JsonCons.JMESPath result: {jsonConsResult}")]
    internal static partial void JsonConsJMESPathResultJsonConsResult(this ILogger logger, JsonElement jsonConsResult);
}
