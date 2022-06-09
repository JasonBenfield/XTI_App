namespace XTI_WebAppClient.Forms;

public class ComplexField : IField
{
    private readonly List<IField> fields = new List<IField>();

    protected ComplexField(string prefix, string name)
    {
        FieldName = string.IsNullOrWhiteSpace(prefix) ? name : $"{prefix}_{name}";
    }

    public string FieldName { get; }
    public string FieldCaption { get; set; } = "";

    protected TField AddField<TField>(TField field) 
        where TField : IField
    {
        fields.Add(field);
        return field;
    }

    public void Export(IDictionary<string, object?> values)
    {
        foreach (var field in fields)
        {
            field.Export(values);
        }
    }

    public void Import(IDictionary<string, object?> values)
    {
        foreach (var field in fields)
        {
            field.Import(values);
        }
    }

    public void Validate(ErrorList errors)
    {
        Validating(errors);
        foreach (var field in fields)
        {
            field.Validate(errors);
        }
    }

    protected virtual void Validating(ErrorList errors) { }

    public object? Value() => null;

    public ErrorModel Error(string message) => new ErrorModel(message, FieldCaption, FieldName);
}