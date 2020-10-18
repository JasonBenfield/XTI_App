using System;
using System.Collections.Generic;
using XTI_Core;

namespace XTI_App.Api
{
    public class ValueTemplateFromType
    {
        public ValueTemplateFromType(Type source)
        {
            if (isDerivedFromSemanticType(source))
            {
                this.source = getSemanticTypeValueType(source);
            }
            else
            {
                this.source = source;
            }
        }

        private readonly Type source;

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
            else if (source == typeof(string))
            {
                valueTemplate = new SimpleValueTemplate(source, true);
            }
            else if (isArrayOrEnumerable(source))
            {
                valueTemplate = new ArrayValueTemplate(source);
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

        private static bool isDerivedFromSemanticType(Type objectType)
        {
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(SemanticType<>))
            {
                return true;
            }
            if (objectType.BaseType == null)
            {
                return false;
            }
            return isDerivedFromSemanticType(objectType.BaseType);
        }

        private static Type getSemanticTypeValueType(Type objectType)
        {
            var propertyInfo = objectType.GetProperty("Value");
            return propertyInfo.PropertyType;
        }
    }
}
