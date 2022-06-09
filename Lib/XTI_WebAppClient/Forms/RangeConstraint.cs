using System.Text;

namespace XTI_WebAppClient.Forms;

public sealed class RangeConstraint<T> : IConstraint<T>
{
    private readonly IComparable? lowerBound;
    private readonly bool isLowerBoundIncluded;
    private readonly IComparable? upperBound;
    private readonly bool isUpperBoundIncluded;

    public RangeConstraint(T lowerBound, bool isLowerBoundIncluded, T upperBound, bool isUpperBoundIncluded)
    {
        this.lowerBound = lowerBound as IComparable;
        this.isLowerBoundIncluded = isLowerBoundIncluded;
        this.upperBound = upperBound as IComparable;
        this.isUpperBoundIncluded = isUpperBoundIncluded;
    }

    public ConstraintResult Test(string caption, T value)
    {
        var message = new StringBuilder();
        if (value is IComparable comparableValue)
        {
            if (lowerBound != null)
            {
                var lowerResult = lowerBound.CompareTo(comparableValue);
                if (lowerResult < 0 || (!isLowerBoundIncluded && lowerResult == 0))
                {
                    var includedText = isLowerBoundIncluded ? "or equal to " : "";
                    message.Append($"{caption} must be greater than {includedText}{lowerResult}");
                }
            }
            if (upperBound != null)
            {
                var upperResult = upperBound.CompareTo(value);
                if (upperResult < 0 || (!isUpperBoundIncluded && upperResult == 0))
                {
                    var includedText = isUpperBoundIncluded ? "or equal to " : "";
                    message.Append($"{caption} must be less than {includedText}{upperResult}");
                }
            }
        }
        return message.Length > 0
            ? ConstraintResult.Failed(message.ToString())
            : ConstraintResult.Passed();
    }

    public ConstraintResult Test(string caption, object value) => Test(caption, (T)value);
}