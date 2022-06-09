namespace XTI_WebAppClient.Forms;

public class Form : ComplexField
{
    protected Form(string name) : base("", name)
    {
    }

    public ErrorList Validate()
    {
        var errors = new ErrorList();
        Validate(errors);
        return errors;
    }

    public IDictionary<string, object?> Export()
    {
        var values = new Dictionary<string, object?>();
        Export(values);
        return values;
    }
}