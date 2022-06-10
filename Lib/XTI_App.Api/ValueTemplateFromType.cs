using XTI_Core;
using XTI_Forms;

namespace XTI_App.Api;

public class ValueTemplateFromType
{
    private readonly Type source;

    public ValueTemplateFromType(Type source)
    {
        this.source = source;
    }

    public ValueTemplate Template()
    {
        ValueTemplate valueTemplate;
        var isNullable = isNullableType(source);
        if (isNullable || source.IsValueType)
        {
            if (isNullable)
            {
                var underlyingType = getNullableType(source);
                if (underlyingType.IsEnum)
                {
                    valueTemplate = new EnumValueTemplate(underlyingType);
                }
                else
                {
                    valueTemplate = new SimpleValueTemplate(underlyingType, true);
                }
            }
            else if (source.IsEnum)
            {
                valueTemplate = new EnumValueTemplate(source);
            }
            else
            {
                valueTemplate = new SimpleValueTemplate(source, false);
            }
        }
        else if (source == typeof(string) || source == typeof(object))
        {
            valueTemplate = new SimpleValueTemplate(source, true);
        }
        else if
        (
            source.IsGenericType &&
            (
                source.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                source.GetGenericTypeDefinition() == typeof(Dictionary<,>)
            )
        )
        {
            valueTemplate = new DictionaryValueTemplate(source);
        }
        else if (isArrayOrEnumerable(source))
        {
            valueTemplate = new ArrayValueTemplate(source);
        }
        else if (isQueryable(source))
        {
            valueTemplate = new QueryableValueTemplate(source);
        }
        else if (isDerivedFromNumericValue(source))
        {
            valueTemplate = new NumericValueTemplate(source);
        }
        else if (typeof(Form).IsAssignableFrom(source))
        {
            var obj = Activator.CreateInstance(source);
            if (obj == null)
            {
                throw new Exception($"Instance should not be null for type {source.GetType()}");
            }
            var form = (Form)obj;
            valueTemplate = new FormValueTemplate(form);
        }
        else
        {
            valueTemplate = new ObjectValueTemplate(source);
        }
        return valueTemplate;
    }

    private static bool isNullableType(Type targetType) =>
        targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>);

    private static Type getNullableType(Type targetType) => targetType.GetGenericArguments()[0];

    private static bool isArrayOrEnumerable(Type targetType) =>
        targetType != typeof(string) &&
        (
            targetType.IsArray ||
            isEnumerable(targetType)
        );

    private static bool isEnumerable(Type targetType) =>
        targetType.IsGenericType &&
        (typeof(IEnumerable<>)).IsAssignableFrom(targetType.GetGenericTypeDefinition());

    private static bool isQueryable(Type targetType) =>
        targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(IQueryable<>);

    private static bool isDerivedFromNumericValue(Type objectType) =>
        typeof(NumericValue).IsAssignableFrom(objectType);
}