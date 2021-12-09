namespace XTI_Forms;

public static class Int32RangeConstraint
{
    public static RangeConstraint<int?> Positive() => RangeConstraint<int?>.Above(0);
    public static RangeConstraint<int?> Negative() => RangeConstraint<int?>.Below(0);

    public static RangeStart<int?> FromOnOrAbove(int? lowerValue) =>
        new RangeStart<int?>(new LowerRangeConstraint<int?>(lowerValue, true));

    public static RangeStart<int?> FromAbove(int? lowerValue) =>
        new RangeStart<int?>(new LowerRangeConstraint<int?>(lowerValue, false));

    public static RangeConstraint<int?> OnOrAbove(int? lowerValue) =>
        new RangeConstraint<int?>(new LowerRangeConstraint<int?>(lowerValue, true));

    public static RangeConstraint<int?> Above(int? lowerValue) =>
        new RangeConstraint<int?>(new LowerRangeConstraint<int?>(lowerValue, false));

    public static RangeConstraint<int?> OnOrBelow(int? upperValue) =>
        new RangeConstraint<int?>(new UpperRangeConstraint<int?>(upperValue, true));

    public static RangeConstraint<int?> Below(int? upperValue) =>
        new RangeConstraint<int?>(new UpperRangeConstraint<int?>(upperValue, false));

}