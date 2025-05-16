using HangulApi.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HangulApi.Configuration;

public class JamoTypeJsonConverter : JsonConverter<JamoType>
{
    public override JamoType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var input = reader.GetString();
        if (string.IsNullOrWhiteSpace(input))
            throw new JsonException("JamoType no puede ser vacío.");

        return JamoType.List.FirstOrDefault(t =>
            string.Equals(t.Name, input, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(t.Value, input, StringComparison.OrdinalIgnoreCase))
            ?? throw new JsonException($"JamoType no reconocido: {input}");
    }

    public override void Write(Utf8JsonWriter writer, JamoType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
