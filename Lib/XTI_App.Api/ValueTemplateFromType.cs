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
                valueTemplate = new SimpleValueTemplate(underlyingType, true);
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

    private static bool isNullableType(Type targetType)
    {
        return targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    private static Type getNullableType(Type targetType)
    {
        return targetType.GetGenericArguments()[0];
    }

    private static bool isArrayOrEnumerable(Type targetType)
    {
        return targetType != typeof(string) && (targetType.IsArray || isEnumerable(targetType));
    }

    private static bool isEnumerable(Type targetType)
    {
        return targetType.IsGenericType && (typeof(IEnumerable<>)).IsAssignableFrom(targetType.GetGenericTypeDefinition());
    }

    private static bool isDerivedFromNumericValue(Type objectType)
    {
        return typeof(NumericValue).IsAssignableFrom(objectType);
    }
}