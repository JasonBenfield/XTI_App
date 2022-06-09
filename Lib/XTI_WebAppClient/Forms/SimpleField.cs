namespace XTI_WebAppClient.Forms;

public class SimpleField<T> : IField
{
    public SimpleField(string prefix, string name)
    {
        FieldName = string.IsNullOrWhiteSpace(prefix) ? name : $"{prefix}_{name}";
    }

    public string FieldName { get; }
    public string FieldCaption { get; set; } = "";
    public T? Value { get; set; }

    object? IField.Value() => Value;

    public void Export(IDictionary<string, object?> values)
    {
        values.Add(FieldName, Value);
    }

    public void Import(IDictionary<string, object?> values)
    {
        if (values.TryGetValue(FieldName, out var value))
        {
            Value = new ConvertedValue<T>(value).Value();
        }
    }

    public ConstraintCollection<T> Constraints { get; } = new ConstraintCollection<T>();

    public void Validate(ErrorList errors)
    {
        Validating(errors);
        Constraints.Validate(errors, this);
    }

    protected virtual void Validating(ErrorList errors) { }

    public ErrorModel Error(string message) => new ErrorModel(message, FieldCaption, FieldName);
}