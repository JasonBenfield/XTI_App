namespace XTI_Forms
{
    public sealed class RangeStart<T>
    {
        private readonly IConstraint<T> lowerConstraint;

        public RangeStart(IConstraint<T> lowerConstraint)
        {
            this.lowerConstraint = lowerConstraint;
        }

        public RangeConstraint<T> ToBelow(T upperValue)
            => new RangeConstraint<T>(lowerConstraint, new UpperRangeConstraint<T>(upperValue, false));

        public RangeConstraint<T> ToOnOrBelow(T upperValue)
            => new RangeConstraint<T>(lowerConstraint, new UpperRangeConstraint<T>(upperValue, true));
    }
}
