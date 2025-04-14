namespace TBAStatReader;

using System.Text;
using System.Text.Json.Nodes;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using TBAAPI.V3Client.Model;

internal static class Extensions
{
    public static JsonNode? GetScoreBreakdownFor(this Match match, string redOrBlue) => JsonNode.Parse(match.ToJson())!["score_breakdown"]?[redOrBlue];

    public static string Serialize(this ChatHistory chatMessages)
    {
        var sb = new StringBuilder();
        foreach (ChatMessageContent message in chatMessages)
        {
            sb.AppendLine($@"{message.Role.Label}: ""{message.Items.OfType<TextContent>().First().Text}""");
        }

        return sb.ToString();
    }
}