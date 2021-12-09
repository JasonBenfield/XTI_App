namespace XTI_Forms;

public sealed class LowerRangeConstraintModel : ConstraintModel
{
    public object Value { get; set; } = new object();
    public bool IsIncluded { get; set; }
}