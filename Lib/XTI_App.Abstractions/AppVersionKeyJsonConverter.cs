using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace XTI_App.Abstractions
{
    public sealed class AppVersionKeyJsonConverter : JsonConverter<AppVersionKey>
    {
        public override AppVersionKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject token");
            }
            var value = "";
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propName = reader.GetString();
                    if (propName == "Value")
                    {
                        reader.Read();
                        value = reader.GetString();
                    }
                }
            }
            return AppVersionKey.Parse(value);
        }

        public override void Write(Utf8JsonWriter writer, AppVersionKey value, JsonSerializerOptions options)
        {
            var writeOptions = new JsonSerializerOptions(options);
            writeOptions.Converters.Remove(this);
            JsonSerializer.Serialize(writer, value, writeOptions);
        }
    }
}
