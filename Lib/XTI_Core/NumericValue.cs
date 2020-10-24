using System;
using System.Linq;

namespace XTI_Core
{
    public abstract class NumericValue : SemanticType<int>, IEquatable<int>
    {
        protected NumericValue(int value, string displayText)
            : base(value, displayText)
        {
        }

        protected bool _EqualsAny(params int[] values) => values.Any(v => Equals(v));
        protected bool _EqualsAny(params NumericValue[] values) => values.Any(v => _Equals(v));

        public override string ToString() => $"{GetType().Name} {Value}: {DisplayText}";
    }
}
