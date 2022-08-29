using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XTI_WebAppClient;

public sealed class ClientNumericValueConverterJsonFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeof(ClientNumericValue).IsAssignableFrom(typeToConvert) &&
        typeToConvert.GetCustomAttribute<JsonConverterAttribute>() == null;

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter)Activator.CreateInstance
        (
            typeof(ClientNumericValueJsonConverter<>).MakeGenericType(new[] { typeToConvert }),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[0],
            culture: null
        )!;
}
