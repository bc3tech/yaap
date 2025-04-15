namespace TBAAPI.V3Client.Api;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;

using JsonCons.JmesPath;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Models.Json;

using TBAAPI.V3Client.Client;
using TBAAPI.V3Client.Model;

internal partial class TeamApi
{
    private ILogger? Log { get; }

    private static readonly JsonDocument EmptyJsonDocument = JsonDocument.Parse("[]");

    public TeamApi(Configuration config, ILogger logger) : this(config) => this.Log = logger;

    /// <summary>
    /// Searches for teams based on a JMESPath expression.
    /// </summary>
    /// <param name="jmesPathExpression">The JMESPath expression used to filter the teams.</param>
    /// <returns>A list of teams that match the JMESPath expression.</returns>
    [KernelFunction, Description("Searches for teams based on a JMESPath expression.")]
    [return: Description("A collection of JSON documents/objects resulting from the JMESPath expression.")]
    public async Task<List<JsonDocument>> SearchTeamsAsync([Description("The query used to filter a JSON document with a single `teams` array of Team objects. Must be a valid JMESPath expression. Use lower-case strings for literals when searching.")] string jmesPathExpression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jmesPathExpression);

        JsonTransformer transformer;
        try
        {
            transformer = JsonTransformer.Parse(jmesPathExpression);
        }
        catch (JmesPathParseException)
        {
            throw new ArgumentException("Invalid JMESPath expression", jmesPathExpression);
        }

        List<JsonDocument> results = [];
        for (var i = 0; ; i++)
        {
            List<Team>? teams = await GetTeamsDetailedAsync(i).ConfigureAwait(false);
            if (teams?.Count is null or 0)
            {
                break;
            }

            JsonElement eltToTransform = JsonSerializer.SerializeToElement(new { teams }, JsonSerialzationOptions.Default);
            JsonDocument filteredTeams = JsonCons.JmesPath.JsonTransformer.Transform(eltToTransform, jmesPathExpression);
            this.Log?.LogTrace("JsonCons.JMESPath result: {jsonConsResult}", filteredTeams.RootElement.ToString());

            if (filteredTeams is not null)
            {
                if ((filteredTeams.RootElement.ValueKind is JsonValueKind.Array && filteredTeams.RootElement.EnumerateArray().Any())
                    || (filteredTeams.RootElement.ValueKind is JsonValueKind.Object && filteredTeams.RootElement.EnumerateObject().Any()))
                {
                    results.Add(filteredTeams);
                }
            }
        }

        this.Log?.LogDebug("Resulting document: {searchResults}", JsonSerializer.Serialize(results));

        return results;
    }

    /// <summary>
    /// Searches for teams within a district based on a JMESPath expression.
    /// </summary>
    /// <param name="districtKey">The key of the district to search within.</param>
    /// <param name="jmesPathExpression">The JMESPath expression used to filter the teams.</param>
    /// <returns>A list of teams that match the JMESPath expression.</returns>
    [KernelFunction, Description("Searches for teams within a district based on a JMESPath expression.")]
    [return: Description("A list of (non-detailed) teams that match the JMESPath expression.")]
    public async Task<List<TeamSimple>> SearchDistrictTeamsAsync(
        [Description("The key of the district to search within.")] string districtKey,
        [Description("The JMESPath expression used to filter the teams.")] string jmesPathExpression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jmesPathExpression);

        List<TeamSimple>? matches = await GetDistrictTeamsAsync(districtKey).ConfigureAwait(false);

        JsonDocument filteredTeams = JsonCons.JmesPath.JsonTransformer.Transform(JsonSerializer.SerializeToElement(matches, JsonSerialzationOptions.Default), jmesPathExpression);
        matches = JsonSerializer.Deserialize<List<TeamSimple>>(filteredTeams, JsonSerialzationOptions.Default) ?? [];

        this.Log?.LogDebug("Resulting document: {searchResults}", JsonSerializer.Serialize(matches));

        return matches;
    }
}
