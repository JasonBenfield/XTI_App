using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace XTI_App.Abstractions;

public sealed class GenericRecordBuilder
{
    private readonly Dictionary<string, object> record = new();

    public GenericRecordBuilder AddProperty(string name, object value)
    {
        record.Add(name, value);
        return this;
    }

    public GenericRecord Build() => new GenericRecord(record);
}

[JsonConverter(typeof(GenericRecordJsonConverter))]
public sealed class GenericRecord
{
    private readonly IDictionary<string, object> record;

    public GenericRecord() : this(new Dictionary<string, object>()) { }

    public GenericRecord(IDictionary<string, object> record)
    {
        this.record = record;
    }

    public bool ContainsField(string fieldName) => record.ContainsKey(fieldName);

    public T ValueOrDefault<T>(string fieldName)
        where T : struct =>
        ValueOrDefault<T>(fieldName, default);

    public T ValueOrDefault<T>(string fieldName, T defaultValue)
    {
        var split = fieldName.Split('.');
        var objs = split.SkipLast(1);
        var parentRecord = this;
        foreach(var obj in objs)
        {
            parentRecord = parentRecord.ValueForInstance(obj, new GenericRecord());
        }
        return parentRecord.ValueForInstance(split.Last(), defaultValue);
    }

    private T ValueForInstance<T>(string fieldName, T defaultValue)
    {
        T value;
        if (record.TryGetValue(fieldName, out var rawValue))
        {
            if (rawValue == null)
            {
                value = defaultValue;
            }
            else if (rawValue.GetType() != typeof(T))
            {
                value = (T)Convert.ChangeType(rawValue, typeof(T));
            }
            else
            {
                value = (T)rawValue;
            }
        }
        else
        {
            value = defaultValue;
        }
        return value;
    }

    public IDictionary<string, object> ToDictionary() =>
        record.ToDictionary(kv => kv.Key, kv => kv.Value);
}

public sealed partial class GenericRecordJsonConverter : JsonConverter<GenericRecord>
{
    public override GenericRecord? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var record = new Dictionary<string, object>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propName = reader.GetString();
                reader.Read();
                var value = new object();
                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var str = reader.GetString() ?? "";
                        if (DateTimeRegex().IsMatch(str))
                        {
                            value = DateTimeOffset.Parse(str);
                        }
                        else
                        {
                            value = str;
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.Number)
                    {
                        value = reader.GetDecimal();
                    }
                    else if (reader.TokenType == JsonTokenType.True)
                    {
                        value = true;
                    }
                    else if (reader.TokenType == JsonTokenType.False)
                    {
                        value = false;
                    }
                    else if(reader.TokenType == JsonTokenType.StartObject)
                    {
                        value = Read(ref reader, typeof(GenericRecord), options) ?? new GenericRecord();
                    }
                }
                record.Add(propName ?? "", value);
            }
        }
        return new GenericRecord(record);
    }

    public override void Write(Utf8JsonWriter writer, GenericRecord value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        var dict = value.ToDictionary();
        foreach (var key in dict.Keys)
        {
            var fieldValue = dict[key];
            if (fieldValue ==  null)
            {
                writer.WriteNull(key);
            }
            else if (fieldValue is string str)
            {
                writer.WriteString(key, str);
            }
            else if (fieldValue is DateTimeOffset dto)
            {
                writer.WriteString(key, dto.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            }
            else if (fieldValue is DateOnly dateOnly)
            {
                writer.WriteString(key, dateOnly.ToString("yyyy-MM-dd"));
            }
            else if (fieldValue is TimeOnly timeOnly)
            {
                writer.WriteString(key, timeOnly.ToString("HH:mm:ss.fff"));
            }
            else if (fieldValue is DateTime dt)
            {
                writer.WriteString(key, dt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            }
            else if (fieldValue is decimal dec)
            {
                writer.WriteNumber(key, dec);
            }
            else if (fieldValue is int intValue)
            {
                writer.WriteNumber(key, intValue);
            }
            else if (fieldValue is long longValue)
            {
                writer.WriteNumber(key, longValue);
            }
            else if (fieldValue is short shortValue)
            {
                writer.WriteNumber(key, shortValue);
            }
            else if (fieldValue is bool boolValue)
            {
                writer.WriteBoolean(key, boolValue);
            }
            else if(fieldValue is GenericRecord record)
            {
                writer.WritePropertyName(key);
                Write(writer, record, options);
            }
        }
        writer.WriteEndObject();
    }

    [GeneratedRegex("\\d{4}-\\d{2}-\\d{2}(T| )?(\\d{2}:\\d{2}(:\\d{2})?(\\.\\d{3}Z)?)?")]
    private static partial Regex DateTimeRegex();
}
