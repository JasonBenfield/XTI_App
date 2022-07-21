namespace XTI_Forms;

public sealed class NotWhiteSpaceConstraint : IConstraint<string>
{
    public ConstraintResult Test(object value) => Test((string)value);

    public ConstraintResult Test(string value)
        => string.IsNullOrWhiteSpace(value)
            ? ConstraintResult.Failed(FormErrors.MustNotBeNullOrWhitespace)
            : ConstraintResult.Passed();

    public ConstraintModel ToModel() => new NotWhiteSpaceConstraintModel
    {
        FailureMessage = FormErrors.MustNotBeNullOrWhitespace
    };
}