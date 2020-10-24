using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTI_Core
{
    public class TextValue : SemanticType<string>
    {
        public TextValue(string value) : base(value)
        {
        }

        public TextValue(string value, string displayText) : base(value, displayText)
        {
        }

        protected bool _EqualsAny(params string[] values) => values.Any(v => Equals(v));
        protected bool _EqualsAny(params TextValue[] values) => values.Any(v => _Equals(v));
        protected override bool _Equals(string other)
        {
            if (Value == null && other == null)
            {
                return true;
            }
            return Value?.Equals(other, StringComparison.OrdinalIgnoreCase) == true;
        }

        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => $"{GetType().Name} {Value}: {DisplayText}";
    }
}
