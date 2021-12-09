namespace XTI_Forms;

public sealed class UpperRangeConstraintModel : ConstraintModel
{
    public object Value { get; set; } = new object();
    public bool IsIncluded { get; set; }
}