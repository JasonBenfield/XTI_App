namespace XTI_WebAppClient.Forms;

public interface IField
{
    string FieldName { get; }
    string FieldCaption { get; }
    object? Value();
    void Validate(ErrorList errors);
    void Import(IDictionary<string, object?> values);
    void Export(IDictionary<string, object?> values);
    ErrorModel Error(string message);
}