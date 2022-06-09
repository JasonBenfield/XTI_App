namespace XTI_WebAppClient.Forms;

public sealed class HiddenField<T> : SimpleField<T>
{
    public HiddenField(string prefix, string name) 
        : base(prefix, name)
    {
    }
}