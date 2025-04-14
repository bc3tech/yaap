namespace TBAAPI.V3Client.Api;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Models.Json;

using TBAAPI.V3Client.Client;
using TBAAPI.V3Client.Model;

public partial class MatchApi
{
    private ILogger? Log { get; }

    private static readonly JsonDocument EmptyJsonDocument = JsonDocument.Parse("[]");

    public MatchApi(Configuration config, ILogger logger) : this(config) => this.Log = logger;

    /// <summary>
    /// Searches for match data based on a JMESPath expression.
    /// </summary>
    /// <param name="jmesPathExpression">The JMESPath expression used to filter matches.</param>
    /// <returns>A list of teams that match the JMESPath expression.</returns>
    [KernelFunction, Description("Searches for match data by team & year based on a JMESPath expression.")]
    [return: Description("A JSON document with the search results.")]
    public async Task<JsonDocument> SearchTeamMatchesByYearAsync(
        [Description("Team Key, eg 'frc254'")] string teamKey,
        int year,
        [Description("The query used to filter a JSON document with a single `matches` array of (non-detailed) Match objects. Must be a valid JMESPath expression. Use case-insensitive syntax for string-based searches.")] string jmesPathExpression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jmesPathExpression);
        if (jmesPathExpression.StartsWith("Matches"))
        {
            jmesPathExpression = jmesPathExpression.Replace("Matches", "matches");
        }

        JsonDocument retVal = EmptyJsonDocument;
        List<MatchSimple>? matches = await GetTeamMatchesByYearAsync(teamKey, year).ConfigureAwait(false);
        if (matches?.Count is not null and not 0)
        {
            JsonElement eltToTransform = JsonSerializer.SerializeToElement(new { matches }, JsonSerialzationOptions.Default);
            JsonDocument filteredMatches = JsonCons.JmesPath.JsonTransformer.Transform(eltToTransform, jmesPathExpression);
            this.Log?.LogTrace("JsonCons.JMESPath result: {jsonConsResult}", filteredMatches.RootElement.ToString());

            if (filteredMatches is not null)
            {
                if ((filteredMatches.RootElement.ValueKind is JsonValueKind.Array && filteredMatches.RootElement.EnumerateArray().Any())
                    || (filteredMatches.RootElement.ValueKind is JsonValueKind.Object && filteredMatches.RootElement.EnumerateObject().Any()))
                {
                    retVal = filteredMatches;
                }
            }
        }

        this.Log?.LogDebug("Resulting document: {searchResults}", JsonSerializer.Serialize(retVal));

        return retVal;
    }

    /// <summary>
    /// Searches for match data based on a JMESPath expression.
    /// </summary>
    /// <param name="jmesPathExpression">The JMESPath expression used to filter matches.</param>
    /// <returns>A list of teams that match the JMESPath expression.</returns>
    [KernelFunction, Description("Searches for match data by event based on a JMESPath expression.")]
    [return: Description("A collection of JSON objects with the search results.")]
    public async Task<JsonDocument> SearchEventMatchesAsync(
        [Description("Event Key, eg '2016nytr'")] string eventKey,
        [Description("The query used to filter a JSON document with a single `matches` array of Match objects. Must be a valid JMESPath expression. Use lower-case strings for literals when searching.")] string jmesPathExpression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jmesPathExpression);
        if (jmesPathExpression.StartsWith("Matches"))
        {
            jmesPathExpression = jmesPathExpression.Replace("Matches", "matches");
        }

        JsonDocument retVal = EmptyJsonDocument;
        List<Match>? matches = await GetEventMatchesDetailedAsync(eventKey).ConfigureAwait(false);
        if (matches?.Count is not null and not 0)
        {
            JsonElement eltToTransform = JsonSerializer.SerializeToElement(new { matches }, JsonSerialzationOptions.Default);
            JsonDocument filteredMatches = JsonCons.JmesPath.JsonTransformer.Transform(eltToTransform, jmesPathExpression);
            this.Log?.LogTrace("JsonCons.JMESPath result: {jsonConsResult}", filteredMatches.RootElement.ToString());

            if (filteredMatches is not null)
            {
                if ((filteredMatches.RootElement.ValueKind is JsonValueKind.Array && filteredMatches.RootElement.EnumerateArray().Any())
                    || (filteredMatches.RootElement.ValueKind is JsonValueKind.Object && filteredMatches.RootElement.EnumerateObject().Any()))
                {
                    retVal = filteredMatches;
                }
            }
        }

        this.Log?.LogDebug("Resulting document: {searchResults}", JsonSerializer.Serialize(retVal));

        return retVal;
    }
}
