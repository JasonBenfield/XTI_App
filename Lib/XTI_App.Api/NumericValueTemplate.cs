﻿using System;
using System.Collections.Generic;
using System.Linq;
using XTI_Core;

namespace XTI_App.Api
{
    public sealed class NumericValueTemplate : ValueTemplate, IEquatable<NumericValueTemplate>
    {
        private readonly string value;
        private readonly int hashCode;

        public NumericValueTemplate(Type dataType)
        {
            DataType = dataType;
            Values = getValues();
            value = $"{DataType}";
            hashCode = value.GetHashCode();
        }

        private NumericValue[] getValues()
        {
            object[] values = null;
            var fieldInfo = DataType.GetField("Values");
            var valuesField = fieldInfo?.GetValue(null);
            if (valuesField != null)
            {
                var methodInfo = valuesField.GetType().GetMethod("All");
                values = (object[])methodInfo?.Invoke(valuesField, new object[] { });
            }
            return (values ?? new object[] { }).Cast<NumericValue>().ToArray();
        }

        public Type DataType { get; }
        public NumericValue[] Values { get; }

        public override int GetHashCode() => hashCode;

        public override bool Equals(object obj)
        {
            if (obj is NumericValueTemplate numTempl)
            {
                return Equals(numTempl);
            }
            return base.Equals(obj);
        }

        public bool Equals(NumericValueTemplate other) => DataType == other?.DataType;

        public IEnumerable<ObjectValueTemplate> ObjectTemplates() => new ObjectValueTemplate[] { };
    }
}
