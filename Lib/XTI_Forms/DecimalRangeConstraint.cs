namespace XTI_Forms;

public static class DecimalRangeConstraint
{
    public static RangeConstraint<decimal?> Positive() => RangeConstraint<decimal?>.Above(0);
    public static RangeConstraint<decimal?> Negative() => RangeConstraint<decimal?>.Below(0);
    public static RangeStart<decimal?> FromOnOrAbove(decimal? lowerValue) =>
        new RangeStart<decimal?>(new LowerRangeConstraint<decimal?>(lowerValue, true));

    public static RangeStart<decimal?> FromAbove(decimal? lowerValue) =>
        new RangeStart<decimal?>(new LowerRangeConstraint<decimal?>(lowerValue, false));

    public static RangeConstraint<decimal?> OnOrAbove(decimal? lowerValue) =>
        new RangeConstraint<decimal?>(new LowerRangeConstraint<decimal?>(lowerValue, true));

    public static RangeConstraint<decimal?> Above(decimal? lowerValue) =>
        new RangeConstraint<decimal?>(new LowerRangeConstraint<decimal?>(lowerValue, false));

    public static RangeConstraint<decimal?> OnOrBelow(decimal? upperValue) =>
        new RangeConstraint<decimal?>(new UpperRangeConstraint<decimal?>(upperValue, true));

    public static RangeConstraint<decimal?> Below(decimal? upperValue) =>
        new RangeConstraint<decimal?>(new UpperRangeConstraint<decimal?>(upperValue, false));
}