namespace XTI_WebAppClient.Forms;

public sealed class InputField<T> : SimpleField<T>
{
    public InputField(string prefix, string name) 
        : base(prefix, name)
    {
    }

    public int? MaxLength { get; set; }
    public bool IsProtected { get; set; }

    protected override void Validating(ErrorList errors)
    {
        if (MaxLength.HasValue)
        {
            var value = Value;
            if ((value?.ToString()?.Length ?? 0) > MaxLength)
            {
                errors.Add(Error($"{FieldCaption} cannot have more than {MaxLength} characters"));
            }
        }
    }
}