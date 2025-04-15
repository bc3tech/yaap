namespace TBAAPI.V3Client.Model;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Microsoft.Extensions.EnumStrings;

using TBAAPI.V3Client.Json;

/// <summary>
/// The competition level the match was played at.
/// </summary>
/// <value>The competition level the match was played at.</value>
[EnumStrings(ExtensionClassModifiers = "public static")]
[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<CompLevelEnum>))]
public enum CompLevelEnum
{
    /// <summary>
    /// Enum Qm for value: qm
    /// </summary>
    [EnumMember(Value = "qm")]
    Qm,

    /// <summary>
    /// Enum Ef for value: ef
    /// </summary>
    [EnumMember(Value = "ef")]
    Ef,

    /// <summary>
    /// Enum Qf for value: qf
    /// </summary>
    [EnumMember(Value = "qf")]
    Qf,

    /// <summary>
    /// Enum Sf for value: sf
    /// </summary>
    [EnumMember(Value = "sf")]
    Sf,

    /// <summary>
    /// Enum F for value: f
    /// </summary>
    [EnumMember(Value = "f")]
    F
}

/// <summary>
/// The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.
/// </summary>
/// <value>The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.</value>
[EnumStrings(ExtensionClassModifiers = "public static")]
[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<WinningAllianceEnum>))]
public enum WinningAllianceEnum
{
    /// <summary>
    /// Enum Red for value: red
    /// </summary>
    [EnumMember(Value = "red")]
    Red,

    /// <summary>
    /// Enum Blue for value: blue
    /// </summary>
    [EnumMember(Value = "blue")]
    Blue,

    /// <summary>
    /// Enum Empty for value: 
    /// </summary>
    [EnumMember(Value = "")]
    Empty
}

/// <summary>
/// String type of the media element.
/// </summary>
/// <value>String type of the media element.</value>
[EnumStrings(ExtensionClassModifiers = "public static")]
[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<TypeEnum>))]
public enum TypeEnum
{
    /// <summary>
    /// Enum Youtube for value: youtube
    /// </summary>
    [EnumMember(Value = "youtube")]
    Youtube,

    /// <summary>
    /// Enum Cdphotothread for value: cdphotothread
    /// </summary>
    [EnumMember(Value = "cdphotothread")]
    Cdphotothread,

    /// <summary>
    /// Enum Imgur for value: imgur
    /// </summary>
    [EnumMember(Value = "imgur")]
    Imgur,

    /// <summary>
    /// Enum FacebookProfile for value: facebook-profile
    /// </summary>
    [EnumMember(Value = "facebook-profile")]
    FacebookProfile,

    /// <summary>
    /// Enum YoutubeChannel for value: youtube-channel
    /// </summary>
    [EnumMember(Value = "youtube-channel")]
    YoutubeChannel,

    /// <summary>
    /// Enum TwitterProfile for value: twitter-profile
    /// </summary>
    [EnumMember(Value = "twitter-profile")]
    TwitterProfile,

    /// <summary>
    /// Enum GithubProfile for value: github-profile
    /// </summary>
    [EnumMember(Value = "github-profile")]
    GithubProfile,

    /// <summary>
    /// Enum InstagramProfile for value: instagram-profile
    /// </summary>
    [EnumMember(Value = "instagram-profile")]
    InstagramProfile,

    /// <summary>
    /// Enum PeriscopeProfile for value: periscope-profile
    /// </summary>
    [EnumMember(Value = "periscope-profile")]
    PeriscopeProfile,

    /// <summary>
    /// Enum Grabcad for value: grabcad
    /// </summary>
    [EnumMember(Value = "grabcad")]
    Grabcad,

    /// <summary>
    /// Enum InstagramImage for value: instagram-image
    /// </summary>
    [EnumMember(Value = "instagram-image")]
    InstagramImage,

    /// <summary>
    /// Enum ExternalLink for value: external-link
    /// </summary>
    [EnumMember(Value = "external-link")]
    ExternalLink,

    /// <summary>
    /// Enum Avatar for value: avatar
    /// </summary>
    [EnumMember(Value = "avatar")]
    Avatar,

    /// <summary>
    /// Enum Twitch for value: twitch
    /// </summary>
    [EnumMember(Value = "twitch")]
    Twitch,

    /// <summary>
    /// Enum Livestream for value: livestream
    /// </summary>
    [EnumMember(Value = "livestream")]
    Livestream,
}