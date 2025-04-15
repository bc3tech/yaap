namespace Models.Json;
using System.Text.Json;

public static class JsonSerialzationOptions
{
    public static JsonSerializerOptions Default { get; } = new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}
