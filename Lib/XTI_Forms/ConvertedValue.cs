namespace XTI_Forms;

public sealed class ConvertedValue<T>
{
    private readonly object? source;

    public ConvertedValue(object? source)
    {
        this.source = source;
    }

    public T? Value()
    {
        if (source == null)
        {
            return default;
        }
        var sourceType = source.GetType();
        var targetType = typeof(T);
        if (IsNullableType(targetType))
        {
            targetType = GetNullableType(targetType);
        }
        if (sourceType == targetType)
        {
            return (T)source;
        }
        if (targetType == typeof(DateOnly) && source is string dateOnlyStr)
        {
            if (string.IsNullOrWhiteSpace(dateOnlyStr)) { return default; }
            return (T)(object)DateOnly.Parse(dateOnlyStr);
        }
        if (targetType == typeof(TimeOnly) && source is string timeOnlyStr)
        {
            if (string.IsNullOrWhiteSpace(timeOnlyStr)) { return default; }
            return (T)(object)TimeOnly.Parse(timeOnlyStr);
        }
        if (targetType == typeof(TimeSpan) && source is string timeSpanStr)
        {
            if (string.IsNullOrWhiteSpace(timeSpanStr)) { return default; }
            return (T)(object)TimeSpan.Parse(timeSpanStr);
        }
        if (targetType == typeof(DateTime) && source is string dateTimeStr)
        {
            if (string.IsNullOrWhiteSpace(dateTimeStr)) { return default; }
            return (T)(object)DateTime.Parse(dateTimeStr);
        }
        if (targetType == typeof(DateTimeOffset) && source is string dateTimeOffsetStr)
        {
            if (string.IsNullOrWhiteSpace(dateTimeOffsetStr)) { return default; }
            return (T)(object)DateTimeOffset.Parse(dateTimeOffsetStr);
        }
        return (T)Convert.ChangeType(source, targetType);
    }

    private static bool IsNullableType(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    private static Type GetNullableType(Type type) => type.GetGenericArguments()[0];
}