using XTI_Core;

namespace XTI_Forms;

public interface IField
{
    string Name { get; }
    void Import(IDictionary<string, object?> values);
    void Export(IDictionary<string, object?> values);
    object? Value();
    void SkipValidation();
    void UnskipValidation();
    void Validate(ErrorList errors);
    ErrorModel Error(string message);
    FieldModel ToModel();
}
public interface IField<T> : IField
{
    void SetValue(T? value);
    new T? Value();
}