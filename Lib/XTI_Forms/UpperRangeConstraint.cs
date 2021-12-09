namespace XTI_Forms;

public sealed class UpperRangeConstraint<T> : IConstraint<T>
{
    private readonly T upperValue;
    private readonly bool isIncluded;

    public UpperRangeConstraint(T upperValue, bool isIncluded)
    {
        this.upperValue = upperValue;
        this.isIncluded = isIncluded;
    }

    ConstraintResult IConstraint.Test(object value) => Test((T)value);

    public ConstraintResult Test(T value)
    {
        return isInRange(value)
            ? ConstraintResult.Passed()
            : ConstraintResult.Failed(failureMessage());
    }

    private bool isInRange(T value)
    {
        if (value is IComparable comparableValue)
        {
            var comparableUpperValue = upperValue as IComparable;
            if(comparableUpperValue == null)
            {
                throw new ArgumentException("upperValue must be IComparable");
            }
            var result = comparableUpperValue.CompareTo(comparableValue);
            if (result > 0 || (isIncluded && result == 0))
            {
                return true;
            }
        }
        return false;
    }

    private string failureMessage()
    {
        var message = isIncluded
            ? FormErrors.UpperRangeInclusive
            : FormErrors.UpperRangeExclusive;
        return string.Format(message, formatValue());
    }

    private string formatValue() => new FormattedValue<T>(upperValue).Format();

    public ConstraintModel ToModel() => new UpperRangeConstraintModel
    {
        Value = upperValue ?? new object(),
        IsIncluded = isIncluded,
        FailureMessage = failureMessage()
    };
}