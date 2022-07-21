using XTI_Core;

namespace XTI_Forms;

public class ComplexField : IField
{
    private readonly List<IField> fields = new ();
    private readonly ConstraintCollection<IField[]> constraints = new();

    protected ComplexField(string prefix, string name)
    {
        Key = new FieldKey(prefix, name);
        Caption = string.Join(" ", new CamelCasedWord(name).Words());
    }

    public FieldKey Key { get; }
    public string Name { get => Key.Name; }
    public string Caption { get; }

    public string TypeName() => GetType().Name;

    public void Import(IDictionary<string, object?> values)
    {
        foreach (var field in fields)
        {
            field.Import(values);
        }
    }

    public void Export(IDictionary<string, object?> values)
    {
        foreach (var field in fields)
        {
            field.Export(values);
        }
    }

    protected InputField<string> AddTextInput(string name)
        => AddInput<string>(name);

    protected InputField<DateTimeOffset?> AddDateInput(string name)
        => AddInput<DateTimeOffset?>(name);

    protected InputField<int?> AddInt32Input(string name)
        => AddInput<int?>(name);

    protected InputField<decimal?> AddDecimalInput(string name)
        => AddInput<decimal?>(name);

    private InputField<T> AddInput<T>(string name)
        => AddField(name, (p, n) => new InputField<T>(p, n));

    protected HiddenField<string> AddTextHidden(string name, string value = "")
        => AddHidden(name, value);

    protected HiddenField<int?> AddInt32Hidden(string name, int? value = null)
        => AddHidden(name, value);

    protected HiddenField<DateTimeOffset?> AddDateHidden(string name, DateTimeOffset? value = null)
        => AddHidden(name, value);

    protected HiddenField<decimal?> AddDecimalHidden(string name, decimal? value = null)
        => AddHidden(name, value);

    private HiddenField<T> AddHidden<T>(string name, T? value = default)
        => AddField(name, (p, n) => new HiddenField<T>(p, n, value));

    protected DropDownField<bool?> AddBooleanDropDown(string name)
        => AddBooleanDropDown(name, "Yes", "No");

    protected DropDownField<bool?> AddBooleanDropDown(string name, string trueText, string falseText)
    {
        return AddDropDown
        (
            name,
            new DropDownItem<bool?>(true, trueText),
            new DropDownItem<bool?>(false, falseText)
        );
    }

    protected DropDownField<string> AddTextDropDown(string name, params DropDownItem<string>[] items)
        => AddDropDown(name, items);

    protected DropDownField<int?> AddInt32DropDown(string name, params DropDownItem<int?>[] items)
        => AddDropDown(name, items);

    protected DropDownField<DateTimeOffset?> AddDateDropDown(string name, params DropDownItem<DateTimeOffset?>[] items)
        => AddDropDown(name, items);

    protected DropDownField<decimal?> AddDecimalDropDown(string name, params DropDownItem<decimal?>[] items)
        => AddDropDown(name, items);

    private DropDownField<T> AddDropDown<T>(string name, params DropDownItem<T>[] items)
    {
        var dropDown = AddField(name, (p, n) => new DropDownField<T>(p, n));
        if (items != null)
        {
            dropDown.AddItems(items);
        }
        return dropDown;
    }

    protected TComplex AddComplex<TComplex>(string name, Func<string, string, TComplex> create)
        where TComplex : ComplexField
        => AddField(name, (p, n) => create(p, n));

    protected TField AddField<TField>(string name, Func<string, string, TField> create)
        where TField : IField
    {
        var complex = create(Key.Value(), name);
        fields.Add(complex);
        return complex;
    }

    public virtual void SetValue(object value)
    {
        Import((Dictionary<string, object?>)value);
    }

    public IEnumerable<IField> Fields() => fields.ToArray();

    public ComplexField AddConstraints(params IConstraint<IField[]>[] constraintsToAdd)
        => AddConstraints((IEnumerable<IConstraint<IField[]>>)constraintsToAdd);

    public ComplexField AddConstraints(IEnumerable<IConstraint<IField[]>> constraintsToAdd)
    {
        constraints.Add(constraintsToAdd);
        return this;
    }

    public void Validate(ErrorList errors)
    {
        Validating(errors);
        foreach (var field in fields)
        {
            field.Validate(errors);
        }
        constraints.Validate(errors, this);
    }

    protected virtual void Validating(ErrorList errors) { }

    public virtual object Value()
    {
        var dict = new Dictionary<string, object?>();
        Export(dict);
        return dict;
    }

    protected IEnumerable<ComplexFieldTemplate> ComplexFieldTemplates()
    {
        var templates = new List<ComplexFieldTemplate>();
        if (!(this is Form))
        {
            var typeTemplate = new ComplexFieldTemplate(TypeName(), fields.ToArray());
            templates.Add(typeTemplate);
        }
        var complexFields = Fields().OfType<ComplexField>();
        templates.AddRange(complexFields.SelectMany(f => f.ComplexFieldTemplates()));
        return templates.Distinct();
    }

    public void SkipValidation()
    {
        foreach (var field in fields)
        {
            field.SkipValidation();
        }
    }

    public void UnskipValidation()
    {
        foreach (var field in fields)
        {
            field.UnskipValidation();
        }
    }

    public ErrorModel Error(string message) => new ErrorModel(message, Caption, Key.Value());

    FieldModel IField.ToModel() => ToModel();

    public ComplexFieldModel ToModel()
    {
        var model = _ToModel();
        model.Name = Key.Name;
        model.Caption = Caption;
        model.TypeName = GetType().Name;
        model.Fields = Fields().Select(f => f.ToModel()).ToArray();
        return model;
    }

    protected virtual ComplexFieldModel _ToModel() => new ComplexFieldModel();
}