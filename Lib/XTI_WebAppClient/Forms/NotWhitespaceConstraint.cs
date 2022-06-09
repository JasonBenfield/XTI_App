namespace XTI_WebAppClient.Forms;

public sealed class NotWhitespaceConstraint : IConstraint<string>
{
    public ConstraintResult Test(string friendlyName, string value)
        => string.IsNullOrWhiteSpace(value)
            ? ConstraintResult.Failed($"{friendlyName} must not be blank")
            : ConstraintResult.Passed();

    public ConstraintResult Test(string friendlyName, object value)
        => Test(friendlyName, (string)value);
}