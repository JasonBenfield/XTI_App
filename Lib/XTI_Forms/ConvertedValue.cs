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
        if (isNullableType(targetType))
        {
            targetType = getNullableType(targetType);
        }
        if (sourceType == targetType)
        {
            return (T)source;
        }
        return (T)Convert.ChangeType(source, targetType);
    }

    private static bool isNullableType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    private static Type getNullableType(Type type)
    {
        return type.GetGenericArguments()[0];
    }
}