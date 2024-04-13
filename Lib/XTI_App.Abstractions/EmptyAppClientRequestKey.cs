namespace XTI_App.Abstractions;

public sealed class EmptyAppClientRequestKey : IAppClientRequestKey
{
    public string Value() => "";
}
