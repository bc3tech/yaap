/* 
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).    A `User-Agent` header may need to be set to prevent a 403 Unauthorized error.
 *
 * The version of the OpenAPI document: 3.8.0
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TBAAPI.V3Client.Model;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Match
/// </summary>
[DataContract]
public partial record Match : IValidatableObject
{
    /// <summary>
    /// The competition level the match was played at.
    /// </summary>
    /// <value>The competition level the match was played at.</value>
    [DataMember(Name = "comp_level", EmitDefaultValue = false), JsonPropertyName("comp_level")]
    public CompLevelEnum CompLevel { get; set; }

    /// <summary>
    /// The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.
    /// </summary>
    /// <value>The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.</value>
    [DataMember(Name = "winning_alliance", EmitDefaultValue = false), JsonPropertyName("winning_alliance")]
    public WinningAllianceEnum? WinningAlliance { get; set; }

    /// <summary>
    /// TBA match key with the format 'yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]', where 'yyyy' is the year, and 'EVENT_CODE' is the event code of the event, 'COMP_LEVEL' is (qm, ef, qf, sf, f), and 'MATCH_NUMBER' is the match number in the competition level. A set number may be appended to the competition level if more than one match in required per set.
    /// </summary>
    /// <value>TBA match key with the format 'yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]', where 'yyyy' is the year, and 'EVENT_CODE' is the event code of the event, 'COMP_LEVEL' is (qm, ef, qf, sf, f), and 'MATCH_NUMBER' is the match number in the competition level. A set number may be appended to the competition level if more than one match in required per set.</value>
    [DataMember(Name = "key", EmitDefaultValue = false), JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// The set number in a series of matches where more than one match is required in the match series.
    /// </summary>
    /// <value>The set number in a series of matches where more than one match is required in the match series.</value>
    [DataMember(Name = "set_number", EmitDefaultValue = false), JsonPropertyName("set_number")]
    public int SetNumber { get; set; }

    /// <summary>
    /// The match number of the match in the competition level.
    /// </summary>
    /// <value>The match number of the match in the competition level.</value>
    [DataMember(Name = "match_number", EmitDefaultValue = false), JsonPropertyName("match_number")]
    public int MatchNumber { get; set; }

    /// <summary>
    /// Gets or Sets Alliances
    /// </summary>
    [DataMember(Name = "alliances", EmitDefaultValue = false), JsonPropertyName("alliances")]
    public MatchSimpleAlliances? Alliances { get; set; }

    /// <summary>
    /// Event key of the event the match was played at.
    /// </summary>
    /// <value>Event key of the event the match was played at.</value>
    [DataMember(Name = "event_key", EmitDefaultValue = false), JsonPropertyName("event_key")]
    public string? EventKey { get; set; }

    /// <summary>
    /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the scheduled match time, as taken from the published schedule.
    /// </summary>
    /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the scheduled match time, as taken from the published schedule.</value>
    [DataMember(Name = "time", EmitDefaultValue = false), JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>
    /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of actual match start time.
    /// </summary>
    /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of actual match start time.</value>
    [DataMember(Name = "actual_time", EmitDefaultValue = false), JsonPropertyName("actual_time")]
    public long? ActualTime { get; set; }

    /// <summary>
    /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the TBA predicted match start time.
    /// </summary>
    /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the TBA predicted match start time.</value>
    [DataMember(Name = "predicted_time", EmitDefaultValue = false), JsonPropertyName("predicted_time")]
    public long? PredictedTime { get; set; }

    /// <summary>
    /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) when the match result was posted.
    /// </summary>
    /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) when the match result was posted.</value>
    [DataMember(Name = "post_result_time", EmitDefaultValue = false), JsonPropertyName("post_result_time")]
    public long? PostResultTime { get; set; }

    /// <summary>
    /// Score breakdown for auto, teleop, etc. points. Varies from year to year. May be null.
    /// </summary>
    /// <value>Score breakdown for auto, teleop, etc. points. Varies from year to year. May be null.</value>
    [DataMember(Name = "score_breakdown", EmitDefaultValue = false), JsonPropertyName("score_breakdown")]
    public object? ScoreBreakdown { get; set; }

    /// <summary>
    /// Array of video objects associated with this match.
    /// </summary>
    /// <value>Array of video objects associated with this match.</value>
    [DataMember(Name = "videos", EmitDefaultValue = false), JsonPropertyName("videos")]
    public IList<MatchVideos>? Videos { get; set; }

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class Match {\n");
        sb.Append("  Key: ").Append(this.Key).Append('\n');
        sb.Append("  CompLevel: ").Append(this.CompLevel).Append('\n');
        sb.Append("  SetNumber: ").Append(this.SetNumber).Append('\n');
        sb.Append("  MatchNumber: ").Append(this.MatchNumber).Append('\n');
        sb.Append("  Alliances: ").Append(this.Alliances).Append('\n');
        sb.Append("  WinningAlliance: ").Append(this.WinningAlliance).Append('\n');
        sb.Append("  EventKey: ").Append(this.EventKey).Append('\n');
        sb.Append("  Time: ").Append(this.Time).Append('\n');
        sb.Append("  ActualTime: ").Append(this.ActualTime).Append('\n');
        sb.Append("  PredictedTime: ").Append(this.PredictedTime).Append('\n');
        sb.Append("  PostResultTime: ").Append(this.PostResultTime).Append('\n');
        sb.Append("  ScoreBreakdown: ").Append(this.ScoreBreakdown).Append('\n');
        sb.Append("  Videos: ").Append(this.Videos).Append('\n');
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public virtual string ToJson() => JsonSerializer.Serialize(this);

    /// <summary>
    /// Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            var hashCode = 41;
            if (this.Key is not null)
            {
                hashCode = (hashCode * 59) + this.Key.GetHashCode();
            }

            hashCode = (hashCode * 59) + this.CompLevel.GetHashCode();
            hashCode = (hashCode * 59) + this.SetNumber.GetHashCode();
            hashCode = (hashCode * 59) + this.MatchNumber.GetHashCode();
            if (this.Alliances is not null)
            {
                hashCode = (hashCode * 59) + this.Alliances.GetHashCode();
            }

            hashCode = (hashCode * 59) + this.WinningAlliance.GetHashCode();
            if (this.EventKey is not null)
            {
                hashCode = (hashCode * 59) + this.EventKey.GetHashCode();
            }

            hashCode = (hashCode * 59) + this.Time.GetHashCode();
            hashCode = (hashCode * 59) + this.ActualTime.GetHashCode();
            hashCode = (hashCode * 59) + this.PredictedTime.GetHashCode();
            hashCode = (hashCode * 59) + this.PostResultTime.GetHashCode();
            if (this.ScoreBreakdown is not null)
            {
                hashCode = (hashCode * 59) + this.ScoreBreakdown.GetHashCode();
            }

            if (this.Videos is not null)
            {
                hashCode = (hashCode * 59) + this.Videos.GetHashCode();
            }

            return hashCode;
        }
    }

    /// <summary>
    /// To validate all properties of the instance
    /// </summary>
    /// <param name="validationContext">Validation context</param>
    /// <returns>Validation Result</returns>
    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) => [];
}
