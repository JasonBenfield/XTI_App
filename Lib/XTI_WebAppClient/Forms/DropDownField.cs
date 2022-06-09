namespace XTI_WebAppClient.Forms;

public sealed class DropDownItem<T>
{
    public DropDownItem(T value, string displayText)
    {
        Value = value;
        DisplayText = displayText;
    }

    public T Value { get; }
    public string DisplayText { get; }
}

public sealed class DropDownField<T> : SimpleField<T>
{
    public DropDownField(string prefix, string name) : base(prefix, name)
    {
    }

    public string ItemCaption { get; set; } = "";
    public IEnumerable<DropDownItem<T>> Items { get; set; } = new DropDownItem<T>[0];
}