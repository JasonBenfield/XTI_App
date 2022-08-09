using XTI_Forms;
using XTI_Core;

namespace XTI_App.Api;

public sealed class FormValueTemplate : ValueTemplate, IEquatable<FormValueTemplate>
{
    private readonly NumericValueTemplate[] numTempls;

    internal FormValueTemplate(Form form)
    {
        Form = form.ToModel();
        DataType = form.GetType();
        var numTemplList = new List<NumericValueTemplate>();
        getNumTempls(numTemplList, DataType);
        numTempls = numTemplList.Distinct().ToArray();
    }

    private void getNumTempls(List<NumericValueTemplate> numTemplList, Type dataType)
    {
        foreach (var prop in dataType.GetProperties())
        {
            var numAttrs = prop.GetCustomAttributes(typeof(NumericValueAttribute), true);
            if (numAttrs.Length > 0)
            {
                numTemplList.Add(new NumericValueTemplate(((NumericValueAttribute)numAttrs[0]).DataType));
            }
            if (typeof(ComplexField).IsAssignableFrom(prop.PropertyType))
            {
                getNumTempls(numTemplList, prop.PropertyType);
            }
        }
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

    public IEnumerable<ObjectValueTemplate> ObjectTemplates() => new ObjectValueTemplate[0];

    public IEnumerable<NumericValueTemplate> NumericValueTemplates() => numTempls;
}