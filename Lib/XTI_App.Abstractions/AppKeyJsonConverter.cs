﻿using System.Text.Json;
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
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        propName = reader.GetString();
                        reader.Read();
                        if (propName == "Value")
                        {
                            name = new AppName(reader.GetString() ?? "");
                        }
                    }
                }
            }
            else if (propName == "Type")
            {
                reader.Read();
                type = (AppType)new NumericValueJsonConverter().Read(ref reader, typeof(AppType), options);
            }
        }
        return new AppKey(name, type);
    }

    public override void Write(Utf8JsonWriter writer, AppKey value, JsonSerializerOptions options)
    {
        var writeOptions = new JsonSerializerOptions(options);
        writeOptions.Converters.Remove(this);
        JsonSerializer.Serialize(writer, value, writeOptions);
    }
}