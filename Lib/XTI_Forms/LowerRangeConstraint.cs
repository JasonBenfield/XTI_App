using System;

namespace XTI_Forms
{
    public sealed class LowerRangeConstraint<T> : IConstraint<T>
    {
        private readonly T lowerValue;
        private readonly bool isIncluded;

        public LowerRangeConstraint(T lowerValue, bool isIncluded)
        {
            this.lowerValue = lowerValue;
            this.isIncluded = isIncluded;
        }

        ConstraintResult IConstraint.Test(object value) => Test((T)value);

        public ConstraintResult Test(T value)
        {
            return isInRange(value)
                ? ConstraintResult.Passed()
                : ConstraintResult.Failed(failureMessage());
        }

        private string failureMessage()
        {
            var message = isIncluded
                ? FormErrors.LowerRangeInclusive
                : FormErrors.LowerRangeExclusive;
            return string.Format(message, formatValue());
        }

        private string formatValue() => new FormattedValue<T>(lowerValue).Format();

        private bool isInRange(T value)
        {
            if (value is IComparable comparableValue)
            {
                var comparableLowerValue = lowerValue as IComparable;
                var result = comparableLowerValue.CompareTo(comparableValue);
                if (result < 0 || (isIncluded && result == 0))
                {
                    return true;
                }
            }
            return false;
        }

        public ConstraintModel ToModel() => new LowerRangeConstraintModel
        {
            Value = lowerValue,
            IsIncluded = isIncluded,
            FailureMessage = failureMessage()
        };
    }
}
