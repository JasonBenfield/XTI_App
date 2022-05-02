using XTI_Forms;

namespace XTI_App.Api;

public sealed class FormValueTemplate : ValueTemplate, IEquatable<FormValueTemplate>
{
    internal FormValueTemplate(Form form)
    {
        Form = form.ToModel();
        DataType = form.GetType();
    }

    public FormModel Form { get; }
    public Type DataType { get; }

    public override int GetHashCode() => DataType.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is FormValueTemplate formTempl)
        {
            return Equals(formTempl);
        }
        return base.Equals(obj);
    }

    public bool Equals(FormValueTemplate? other) => DataType.Equals(other?.DataType);

    public IEnumerable<ObjectValueTemplate> ObjectTemplates() => new ObjectValueTemplate[] { };
}