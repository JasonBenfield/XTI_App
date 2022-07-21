namespace XTI_Forms;

public sealed class FieldsMustBeEqualConstraint : IConstraint<string>
{
    private readonly IField<string> otherField;

    public FieldsMustBeEqualConstraint(IField<string> otherField)
    {
        this.otherField = otherField;
    }

    public ConstraintResult Test(object value) => Test((string)value);

    public ConstraintResult Test(string value)
    {
        var otherValue = otherField.Value();
        return value == otherValue
            ? ConstraintResult.Passed()
            : ConstraintResult.Failed(string.Format(FormErrors.FieldsMustBeEqual, otherField.ToModel().Caption));
    }

    public ConstraintModel ToModel() => new FieldsMustBeEqualModel();
}

public sealed class FieldsMustBeEqualModel : ConstraintModel
{
}
