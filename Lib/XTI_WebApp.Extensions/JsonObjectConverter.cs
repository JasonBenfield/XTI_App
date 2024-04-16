using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace XTI_WebApp.Extensions;

public sealed partial class JsonObjectConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        var converter = options.GetConverter(typeof(JsonElement)) as JsonConverter<JsonElement>;
        if (converter == null)
        {
            throw new JsonException();
        }
        var jsonEl = converter.Read(ref reader, type, options);
        return DeserializeValue(jsonEl, options);
    }

    private static object? DeserializeValue(JsonElement jsonEl, JsonSerializerOptions options)
    {
        object? deserializedValue;
        if (jsonEl.ValueKind == JsonValueKind.True)
        {
            deserializedValue = true;
        }
        else if (jsonEl.ValueKind == JsonValueKind.False)
        {
            deserializedValue = false;
        }
        else if (jsonEl.ValueKind == JsonValueKind.Number)
        {
            if (jsonEl.TryGetDecimal(out var numberValue))
            {
                deserializedValue = numberValue;
            }
            else
            {
                deserializedValue = null;
            }
        }
        else if (jsonEl.ValueKind == JsonValueKind.String)
        {
            var dateTimeText = jsonEl.GetString() ?? "";
            if (DateOnlyRegex().IsMatch(dateTimeText))
            {
                deserializedValue = DateOnly.Parse(dateTimeText);
            }
            else if (DateTimeRegex().IsMatch(dateTimeText))
            {
                deserializedValue = DateTimeOffset.Parse(dateTimeText);
            }
            else if (TimeOnlyRegex().IsMatch(dateTimeText))
            {
                deserializedValue = TimeOnly.Parse(dateTimeText);
            }
            else
            {
                deserializedValue = jsonEl.GetString();
            }
        }
        else
        {
            var converter = options.GetConverter(typeof(JsonElement)) as JsonConverter<JsonElement>;
            if (converter == null)
            {
                throw new JsonException();
            }
            if (jsonEl.ValueKind == JsonValueKind.Object)
            {
                deserializedValue = JsonSerializer.Deserialize<ExpandoObject>(jsonEl.GetRawText(), options);
            }
            else if (jsonEl.ValueKind == JsonValueKind.Array)
            {
                deserializedValue = DeserializeArray(jsonEl, options);
            }
            else
            {
                deserializedValue = null;
            }
        }
        return deserializedValue;
    }

    private static List<object?> DeserializeArray(JsonElement jsonEl, JsonSerializerOptions options)
    {
        var list = new List<object?>();
        foreach (var arrEl in jsonEl.EnumerateArray())
        {
            var deserializedValue = DeserializeValue(arrEl, options);
            list.Add(deserializedValue);
        }
        return list;
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        throw new InvalidOperationException("Directly writing object not supported");
    }

    [GeneratedRegex("^\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}\\.\\d{3,7}Z$")]
    private static partial Regex DateTimeRegex();

    [GeneratedRegex("^\\d{2}:\\d{2}:\\d{2}\\.\\d{3,7}$")]
    private static partial Regex TimeOnlyRegex();

    [GeneratedRegex("^\\d{4}-\\d{2}-\\d{2}$")]
    private static partial Regex DateOnlyRegex();
}