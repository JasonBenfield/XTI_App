using XTI_Core;

namespace XTI_Forms;

public class Form : ComplexField
{
    protected Form(string name) : base("", name)
    {
    }

    public IDictionary<string, object> Export()
    {
        var exported = new Dictionary<string, object>();
        Export(exported);
        return exported;
    }

    protected sealed override void Validating(ErrorList errors)
    {
        UnskipValidation();
        ValidatingForm(errors);
    }

    protected virtual void ValidatingForm(ErrorList errors) { }

    public new FormModel ToModel() => (FormModel)base.ToModel();

    protected override ComplexFieldModel _ToModel() => new FormModel()
    {
        ComplexFieldTemplates = ComplexFieldTemplates().ToArray()
    };
}