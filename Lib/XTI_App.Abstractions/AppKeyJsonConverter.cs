using System.Text.Json;
using System.Text.Json.Serialization;
using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class AppKeyJsonConverter : JsonConverter<AppKey>
{
    public override AppKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return AppKey.Parse(reader.GetString() ?? "");
        }
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }
        var name = AppName.Unknown;
        var type = AppType.Values.NotFound;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            var propName = reader.GetString();
            if (propName == "Name")
            {
                reader.Read();
                name = new TextValueJsonConverter<AppName>().Read(ref reader, typeof(AppName), options)
                    ?? AppName.Unknown;
            }
            else if (propName == "Type")
            {
                reader.Read();
                type = new NumericValueJsonConverter<AppType>().Read(ref reader, typeof(AppType), options)
                    ?? AppType.Values.GetDefault();
            }
        }
        return new AppKey(name, type);
    }

    public override void Write(Utf8JsonWriter writer, AppKey value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Name");
        new TextValueJsonConverter<AppName>().Write(writer, value.Name, options);
        writer.WritePropertyName("Type");
        new NumericValueJsonConverter<AppType>().Write(writer, value.Type, options);
        writer.WriteEndObject();
    }
}