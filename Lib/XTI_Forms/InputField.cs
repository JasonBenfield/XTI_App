using XTI_Core;

namespace XTI_Forms;

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
            var value = Value();
            if ((value?.ToString()?.Length ?? 0) > MaxLength)
            {
                errors.Add(Error(string.Format(FormErrors.MustNotExceedLength, MaxLength)));
            }
        }
    }

    public new InputFieldModel ToModel() => (InputFieldModel)base.ToModel();

    protected override SimpleFieldModel _ToModel() => new InputFieldModel
    {
        IsProtected = IsProtected,
        MaxLength = MaxLength
    };
}