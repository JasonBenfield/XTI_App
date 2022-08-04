namespace XTI_Forms;

public static class FormExtensions
{
    public static SimpleField<string> MustNotBeWhiteSpace(this SimpleField<string> field) =>
        field.AddConstraints(new NotWhiteSpaceConstraint()); 

    public static SimpleField<string> MustBeEqualToField(this SimpleField<string> field, IField<string> otherField) =>
        field.AddConstraints(new FieldsMustBeEqualConstraint(otherField));

    public static SimpleField<int?> MustBePositive(this SimpleField<int?> field) => field.MustBeInRange(RangeConstraint<int?>.Above(0));

    public static SimpleField<int?> MustBeNegative(this SimpleField<int?> field) => field.MustBeInRange(RangeConstraint<int?>.Below(0));

    public static SimpleField<int?> MustBeOnOrAbove(this SimpleField<int?> field, int? lowerValue) =>
        field.MustBeInRange(new RangeConstraint<int?>(new LowerRangeConstraint<int?>(lowerValue, true)));

    public static SimpleField<int?> MustBeAbove(this SimpleField<int?> field, int? lowerValue) =>
        field.MustBeInRange(new RangeConstraint<int?>(new LowerRangeConstraint<int?>(lowerValue, false)));

    public static SimpleField<int?> MustBeOnOrBelow(this SimpleField<int?> field, int? upperValue) =>
        field.MustBeInRange(new RangeConstraint<int?>(new UpperRangeConstraint<int?>(upperValue, true)));

    public static SimpleField<int?> MustBeBelow(this SimpleField<int?> field, int? upperValue) =>
        field.MustBeInRange(new RangeConstraint<int?>(new UpperRangeConstraint<int?>(upperValue, false)));

    public static SimpleField<int?> MustBeInRange(this SimpleField<int?> field, RangeConstraint<int?> constraint) =>
        field.AddConstraints(constraint);

}
