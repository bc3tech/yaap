namespace ApiClient;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class ToLowerStringValuesJsonConverter : JsonConverter<string>
{
    public static ToLowerStringValuesJsonConverter Instance = new();
    private ToLowerStringValuesJsonConverter() { }

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetString()?.ToLowerInvariant();

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => writer.WriteStringValue(value);
}
