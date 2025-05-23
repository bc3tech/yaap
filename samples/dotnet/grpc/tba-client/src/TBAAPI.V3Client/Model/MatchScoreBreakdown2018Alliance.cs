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
/// MatchScoreBreakdown2018Alliance
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MatchScoreBreakdown2018Alliance" /> class.
/// </remarks>
[DataContract]
public partial record MatchScoreBreakdown2018Alliance : IValidatableObject
{

    /// <summary>
    /// Gets or Sets AdjustPoints
    /// </summary>
    [DataMember(Name = "adjustPoints", EmitDefaultValue = false), JsonPropertyName("adjustPoints")]
    public int AdjustPoints { get; set; }

    /// <summary>
    /// Gets or Sets AutoOwnershipPoints
    /// </summary>
    [DataMember(Name = "autoOwnershipPoints", EmitDefaultValue = false), JsonPropertyName("autoOwnershipPoints")]
    public int AutoOwnershipPoints { get; set; }

    /// <summary>
    /// Gets or Sets AutoPoints
    /// </summary>
    [DataMember(Name = "autoPoints", EmitDefaultValue = false), JsonPropertyName("autoPoints")]
    public int AutoPoints { get; set; }

    /// <summary>
    /// Gets or Sets AutoQuestRankingPoint
    /// </summary>
    [DataMember(Name = "autoQuestRankingPoint", EmitDefaultValue = false), JsonPropertyName("autoQuestRankingPoint")]
    public bool AutoQuestRankingPoint { get; set; }

    /// <summary>
    /// Gets or Sets AutoRobot1
    /// </summary>
    [DataMember(Name = "autoRobot1", EmitDefaultValue = false), JsonPropertyName("autoRobot1")]
    public string? AutoRobot1 { get; set; }

    /// <summary>
    /// Gets or Sets AutoRobot2
    /// </summary>
    [DataMember(Name = "autoRobot2", EmitDefaultValue = false), JsonPropertyName("autoRobot2")]
    public string? AutoRobot2 { get; set; }

    /// <summary>
    /// Gets or Sets AutoRobot3
    /// </summary>
    [DataMember(Name = "autoRobot3", EmitDefaultValue = false), JsonPropertyName("autoRobot3")]
    public string? AutoRobot3 { get; set; }

    /// <summary>
    /// Gets or Sets AutoRunPoints
    /// </summary>
    [DataMember(Name = "autoRunPoints", EmitDefaultValue = false), JsonPropertyName("autoRunPoints")]
    public int AutoRunPoints { get; set; }

    /// <summary>
    /// Gets or Sets AutoScaleOwnershipSec
    /// </summary>
    [DataMember(Name = "autoScaleOwnershipSec", EmitDefaultValue = false), JsonPropertyName("autoScaleOwnershipSec")]
    public int AutoScaleOwnershipSec { get; set; }

    /// <summary>
    /// Gets or Sets AutoSwitchAtZero
    /// </summary>
    [DataMember(Name = "autoSwitchAtZero", EmitDefaultValue = false), JsonPropertyName("autoSwitchAtZero")]
    public bool AutoSwitchAtZero { get; set; }

    /// <summary>
    /// Gets or Sets AutoSwitchOwnershipSec
    /// </summary>
    [DataMember(Name = "autoSwitchOwnershipSec", EmitDefaultValue = false), JsonPropertyName("autoSwitchOwnershipSec")]
    public int AutoSwitchOwnershipSec { get; set; }

    /// <summary>
    /// Gets or Sets EndgamePoints
    /// </summary>
    [DataMember(Name = "endgamePoints", EmitDefaultValue = false), JsonPropertyName("endgamePoints")]
    public int EndgamePoints { get; set; }

    /// <summary>
    /// Gets or Sets EndgameRobot1
    /// </summary>
    [DataMember(Name = "endgameRobot1", EmitDefaultValue = false), JsonPropertyName("endgameRobot1")]
    public string? EndgameRobot1 { get; set; }

    /// <summary>
    /// Gets or Sets EndgameRobot2
    /// </summary>
    [DataMember(Name = "endgameRobot2", EmitDefaultValue = false), JsonPropertyName("endgameRobot2")]
    public string? EndgameRobot2 { get; set; }

    /// <summary>
    /// Gets or Sets EndgameRobot3
    /// </summary>
    [DataMember(Name = "endgameRobot3", EmitDefaultValue = false), JsonPropertyName("endgameRobot3")]
    public string? EndgameRobot3 { get; set; }

    /// <summary>
    /// Gets or Sets FaceTheBossRankingPoint
    /// </summary>
    [DataMember(Name = "faceTheBossRankingPoint", EmitDefaultValue = false), JsonPropertyName("faceTheBossRankingPoint")]
    public bool FaceTheBossRankingPoint { get; set; }

    /// <summary>
    /// Gets or Sets FoulCount
    /// </summary>
    [DataMember(Name = "foulCount", EmitDefaultValue = false), JsonPropertyName("foulCount")]
    public int FoulCount { get; set; }

    /// <summary>
    /// Gets or Sets FoulPoints
    /// </summary>
    [DataMember(Name = "foulPoints", EmitDefaultValue = false), JsonPropertyName("foulPoints")]
    public int FoulPoints { get; set; }

    /// <summary>
    /// Gets or Sets Rp
    /// </summary>
    [DataMember(Name = "rp", EmitDefaultValue = false), JsonPropertyName("rp")]
    public int Rp { get; set; }

    /// <summary>
    /// Gets or Sets TechFoulCount
    /// </summary>
    [DataMember(Name = "techFoulCount", EmitDefaultValue = false), JsonPropertyName("techFoulCount")]
    public int TechFoulCount { get; set; }

    /// <summary>
    /// Gets or Sets TeleopOwnershipPoints
    /// </summary>
    [DataMember(Name = "teleopOwnershipPoints", EmitDefaultValue = false), JsonPropertyName("teleopOwnershipPoints")]
    public int TeleopOwnershipPoints { get; set; }

    /// <summary>
    /// Gets or Sets TeleopPoints
    /// </summary>
    [DataMember(Name = "teleopPoints", EmitDefaultValue = false), JsonPropertyName("teleopPoints")]
    public int TeleopPoints { get; set; }

    /// <summary>
    /// Gets or Sets TeleopScaleBoostSec
    /// </summary>
    [DataMember(Name = "teleopScaleBoostSec", EmitDefaultValue = false), JsonPropertyName("teleopScaleBoostSec")]
    public int TeleopScaleBoostSec { get; set; }

    /// <summary>
    /// Gets or Sets TeleopScaleForceSec
    /// </summary>
    [DataMember(Name = "teleopScaleForceSec", EmitDefaultValue = false), JsonPropertyName("teleopScaleForceSec")]
    public int TeleopScaleForceSec { get; set; }

    /// <summary>
    /// Gets or Sets TeleopScaleOwnershipSec
    /// </summary>
    [DataMember(Name = "teleopScaleOwnershipSec", EmitDefaultValue = false), JsonPropertyName("teleopScaleOwnershipSec")]
    public int TeleopScaleOwnershipSec { get; set; }

    /// <summary>
    /// Gets or Sets TeleopSwitchBoostSec
    /// </summary>
    [DataMember(Name = "teleopSwitchBoostSec", EmitDefaultValue = false), JsonPropertyName("teleopSwitchBoostSec")]
    public int TeleopSwitchBoostSec { get; set; }

    /// <summary>
    /// Gets or Sets TeleopSwitchForceSec
    /// </summary>
    [DataMember(Name = "teleopSwitchForceSec", EmitDefaultValue = false), JsonPropertyName("teleopSwitchForceSec")]
    public int TeleopSwitchForceSec { get; set; }

    /// <summary>
    /// Gets or Sets TeleopSwitchOwnershipSec
    /// </summary>
    [DataMember(Name = "teleopSwitchOwnershipSec", EmitDefaultValue = false), JsonPropertyName("teleopSwitchOwnershipSec")]
    public int TeleopSwitchOwnershipSec { get; set; }

    /// <summary>
    /// Gets or Sets TotalPoints
    /// </summary>
    [DataMember(Name = "totalPoints", EmitDefaultValue = false), JsonPropertyName("totalPoints")]
    public int TotalPoints { get; set; }

    /// <summary>
    /// Gets or Sets VaultBoostPlayed
    /// </summary>
    [DataMember(Name = "vaultBoostPlayed", EmitDefaultValue = false), JsonPropertyName("vaultBoostPlayed")]
    public int VaultBoostPlayed { get; set; }

    /// <summary>
    /// Gets or Sets VaultBoostTotal
    /// </summary>
    [DataMember(Name = "vaultBoostTotal", EmitDefaultValue = false), JsonPropertyName("vaultBoostTotal")]
    public int VaultBoostTotal { get; set; }

    /// <summary>
    /// Gets or Sets VaultForcePlayed
    /// </summary>
    [DataMember(Name = "vaultForcePlayed", EmitDefaultValue = false), JsonPropertyName("vaultForcePlayed")]
    public int VaultForcePlayed { get; set; }

    /// <summary>
    /// Gets or Sets VaultForceTotal
    /// </summary>
    [DataMember(Name = "vaultForceTotal", EmitDefaultValue = false), JsonPropertyName("vaultForceTotal")]
    public int VaultForceTotal { get; set; }

    /// <summary>
    /// Gets or Sets VaultLevitatePlayed
    /// </summary>
    [DataMember(Name = "vaultLevitatePlayed", EmitDefaultValue = false), JsonPropertyName("vaultLevitatePlayed")]
    public int VaultLevitatePlayed { get; set; }

    /// <summary>
    /// Gets or Sets VaultLevitateTotal
    /// </summary>
    [DataMember(Name = "vaultLevitateTotal", EmitDefaultValue = false), JsonPropertyName("vaultLevitateTotal")]
    public int VaultLevitateTotal { get; set; }

    /// <summary>
    /// Gets or Sets VaultPoints
    /// </summary>
    [DataMember(Name = "vaultPoints", EmitDefaultValue = false), JsonPropertyName("vaultPoints")]
    public int VaultPoints { get; set; }

    /// <summary>
    /// Unofficial TBA-computed value of the FMS provided GameData given to the alliance teams at the start of the match. 3 Character String containing 'L' and 'R' only. The first character represents the near switch, the 2nd the scale, and the 3rd the far, opposing, switch from the alliance&#39;s perspective. An 'L' in a position indicates the platform on the left will be lit for the alliance while an 'R' will indicate the right platform will be lit for the alliance. See also [WPI Screen Steps](https://wpilib.screenstepslive.com/s/currentCS/m/getting_started/l/826278-2018-game-data-details).
    /// </summary>
    /// <value>Unofficial TBA-computed value of the FMS provided GameData given to the alliance teams at the start of the match. 3 Character String containing 'L' and 'R' only. The first character represents the near switch, the 2nd the scale, and the 3rd the far, opposing, switch from the alliance&#39;s perspective. An 'L' in a position indicates the platform on the left will be lit for the alliance while an 'R' will indicate the right platform will be lit for the alliance. See also [WPI Screen Steps](https://wpilib.screenstepslive.com/s/currentCS/m/getting_started/l/826278-2018-game-data-details).</value>
    [DataMember(Name = "tba_gameData", EmitDefaultValue = false), JsonPropertyName("tba_gameData")]
    public string? TbaGameData { get; set; }

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class MatchScoreBreakdown2018Alliance {\n");
        sb.Append("  AdjustPoints: ").Append(this.AdjustPoints).Append('\n');
        sb.Append("  AutoOwnershipPoints: ").Append(this.AutoOwnershipPoints).Append('\n');
        sb.Append("  AutoPoints: ").Append(this.AutoPoints).Append('\n');
        sb.Append("  AutoQuestRankingPoint: ").Append(this.AutoQuestRankingPoint).Append('\n');
        sb.Append("  AutoRobot1: ").Append(this.AutoRobot1).Append('\n');
        sb.Append("  AutoRobot2: ").Append(this.AutoRobot2).Append('\n');
        sb.Append("  AutoRobot3: ").Append(this.AutoRobot3).Append('\n');
        sb.Append("  AutoRunPoints: ").Append(this.AutoRunPoints).Append('\n');
        sb.Append("  AutoScaleOwnershipSec: ").Append(this.AutoScaleOwnershipSec).Append('\n');
        sb.Append("  AutoSwitchAtZero: ").Append(this.AutoSwitchAtZero).Append('\n');
        sb.Append("  AutoSwitchOwnershipSec: ").Append(this.AutoSwitchOwnershipSec).Append('\n');
        sb.Append("  EndgamePoints: ").Append(this.EndgamePoints).Append('\n');
        sb.Append("  EndgameRobot1: ").Append(this.EndgameRobot1).Append('\n');
        sb.Append("  EndgameRobot2: ").Append(this.EndgameRobot2).Append('\n');
        sb.Append("  EndgameRobot3: ").Append(this.EndgameRobot3).Append('\n');
        sb.Append("  FaceTheBossRankingPoint: ").Append(this.FaceTheBossRankingPoint).Append('\n');
        sb.Append("  FoulCount: ").Append(this.FoulCount).Append('\n');
        sb.Append("  FoulPoints: ").Append(this.FoulPoints).Append('\n');
        sb.Append("  Rp: ").Append(this.Rp).Append('\n');
        sb.Append("  TechFoulCount: ").Append(this.TechFoulCount).Append('\n');
        sb.Append("  TeleopOwnershipPoints: ").Append(this.TeleopOwnershipPoints).Append('\n');
        sb.Append("  TeleopPoints: ").Append(this.TeleopPoints).Append('\n');
        sb.Append("  TeleopScaleBoostSec: ").Append(this.TeleopScaleBoostSec).Append('\n');
        sb.Append("  TeleopScaleForceSec: ").Append(this.TeleopScaleForceSec).Append('\n');
        sb.Append("  TeleopScaleOwnershipSec: ").Append(this.TeleopScaleOwnershipSec).Append('\n');
        sb.Append("  TeleopSwitchBoostSec: ").Append(this.TeleopSwitchBoostSec).Append('\n');
        sb.Append("  TeleopSwitchForceSec: ").Append(this.TeleopSwitchForceSec).Append('\n');
        sb.Append("  TeleopSwitchOwnershipSec: ").Append(this.TeleopSwitchOwnershipSec).Append('\n');
        sb.Append("  TotalPoints: ").Append(this.TotalPoints).Append('\n');
        sb.Append("  VaultBoostPlayed: ").Append(this.VaultBoostPlayed).Append('\n');
        sb.Append("  VaultBoostTotal: ").Append(this.VaultBoostTotal).Append('\n');
        sb.Append("  VaultForcePlayed: ").Append(this.VaultForcePlayed).Append('\n');
        sb.Append("  VaultForceTotal: ").Append(this.VaultForceTotal).Append('\n');
        sb.Append("  VaultLevitatePlayed: ").Append(this.VaultLevitatePlayed).Append('\n');
        sb.Append("  VaultLevitateTotal: ").Append(this.VaultLevitateTotal).Append('\n');
        sb.Append("  VaultPoints: ").Append(this.VaultPoints).Append('\n');
        sb.Append("  TbaGameData: ").Append(this.TbaGameData).Append('\n');
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public virtual string ToJson() => JsonSerializer.Serialize(this, GetType());

    /// <summary>
    /// Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            var hashCode = 41;
            hashCode = (hashCode * 59) + this.AdjustPoints.GetHashCode();
            hashCode = (hashCode * 59) + this.AutoOwnershipPoints.GetHashCode();
            hashCode = (hashCode * 59) + this.AutoPoints.GetHashCode();
            hashCode = (hashCode * 59) + this.AutoQuestRankingPoint.GetHashCode();
            if (this.AutoRobot1 is not null)
            {
                hashCode = (hashCode * 59) + this.AutoRobot1.GetHashCode();
            }

            if (this.AutoRobot2 is not null)
            {
                hashCode = (hashCode * 59) + this.AutoRobot2.GetHashCode();
            }

            if (this.AutoRobot3 is not null)
            {
                hashCode = (hashCode * 59) + this.AutoRobot3.GetHashCode();
            }

            hashCode = (hashCode * 59) + this.AutoRunPoints.GetHashCode();
            hashCode = (hashCode * 59) + this.AutoScaleOwnershipSec.GetHashCode();
            hashCode = (hashCode * 59) + this.AutoSwitchAtZero.GetHashCode();
            hashCode = (hashCode * 59) + this.AutoSwitchOwnershipSec.GetHashCode();
            hashCode = (hashCode * 59) + this.EndgamePoints.GetHashCode();
            if (this.EndgameRobot1 is not null)
            {
                hashCode = (hashCode * 59) + this.EndgameRobot1.GetHashCode();
            }

            if (this.EndgameRobot2 is not null)
            {
                hashCode = (hashCode * 59) + this.EndgameRobot2.GetHashCode();
            }

            if (this.EndgameRobot3 is not null)
            {
                hashCode = (hashCode * 59) + this.EndgameRobot3.GetHashCode();
            }

            hashCode = (hashCode * 59) + this.FaceTheBossRankingPoint.GetHashCode();
            hashCode = (hashCode * 59) + this.FoulCount.GetHashCode();
            hashCode = (hashCode * 59) + this.FoulPoints.GetHashCode();
            hashCode = (hashCode * 59) + this.Rp.GetHashCode();
            hashCode = (hashCode * 59) + this.TechFoulCount.GetHashCode();
            hashCode = (hashCode * 59) + this.TeleopOwnershipPoints.GetHashCode();
            hashCode = (hashCode * 59) + this.TeleopPoints.GetHashCode();
            hashCode = (hashCode * 59) + this.TeleopScaleBoostSec.GetHashCode();
            hashCode = (hashCode * 59) + this.TeleopScaleForceSec.GetHashCode();
            hashCode = (hashCode * 59) + this.TeleopScaleOwnershipSec.GetHashCode();
            hashCode = (hashCode * 59) + this.TeleopSwitchBoostSec.GetHashCode();
            hashCode = (hashCode * 59) + this.TeleopSwitchForceSec.GetHashCode();
            hashCode = (hashCode * 59) + this.TeleopSwitchOwnershipSec.GetHashCode();
            hashCode = (hashCode * 59) + this.TotalPoints.GetHashCode();
            hashCode = (hashCode * 59) + this.VaultBoostPlayed.GetHashCode();
            hashCode = (hashCode * 59) + this.VaultBoostTotal.GetHashCode();
            hashCode = (hashCode * 59) + this.VaultForcePlayed.GetHashCode();
            hashCode = (hashCode * 59) + this.VaultForceTotal.GetHashCode();
            hashCode = (hashCode * 59) + this.VaultLevitatePlayed.GetHashCode();
            hashCode = (hashCode * 59) + this.VaultLevitateTotal.GetHashCode();
            hashCode = (hashCode * 59) + this.VaultPoints.GetHashCode();
            if (this.TbaGameData is not null)
            {
                hashCode = (hashCode * 59) + this.TbaGameData.GetHashCode();
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
