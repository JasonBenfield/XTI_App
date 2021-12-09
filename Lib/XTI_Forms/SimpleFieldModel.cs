namespace XTI_Forms;

public class SimpleFieldModel : FieldModel
{
    public object? Value { get; set; }
    public bool IsNullAllowed { get; set; }
    public Type? InputDataType { get; set; }
    public ConstraintModel[] Constraints { get; set; } = new ConstraintModel[0];
}