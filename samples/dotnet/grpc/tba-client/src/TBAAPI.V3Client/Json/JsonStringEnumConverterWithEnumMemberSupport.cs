namespace TBAAPI.V3Client.Json;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonStringEnumConverterWithEnumMemberSupport<T> : JsonConverter<T> where T : Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.String
            || typeToConvert != typeof(T))
        {
            throw new JsonException();
        }

        var enumString = reader.GetString();
        if (string.IsNullOrEmpty(enumString))
        {
            return default!;
        }

        Type enumType = typeof(T);
        System.Reflection.MemberInfo? enumMember = (enumType.GetMembers().FirstOrDefault(m => m.GetCustomAttributes(typeof(System.Runtime.Serialization.EnumMemberAttribute), false).Any(a => ((System.Runtime.Serialization.EnumMemberAttribute)a).Value == enumString))
            ?? enumType.GetMembers().FirstOrDefault(m => m.Name.Equals(enumString, options.PropertyNameCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)))
            ?? throw new JsonException($@"Could not map JSON value '{enumString}' to enum type '{enumType.Name}'.");

        return (T)Enum.Parse(enumType, enumMember.Name);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        System.Reflection.MemberInfo enumMember = typeof(T).GetMember(value.ToString()).First();
        switch (enumMember.GetCustomAttributes(typeof(System.Runtime.Serialization.EnumMemberAttribute), false).FirstOrDefault())
        {
            case System.Runtime.Serialization.EnumMemberAttribute enumMemberAttribute:
                writer.WriteStringValue(enumMemberAttribute.Value);
                break;
            default:
                writer.WriteStringValue(value.ToString("G"));
                break;
        }
    }
}
