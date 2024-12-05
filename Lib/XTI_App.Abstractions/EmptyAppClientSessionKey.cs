namespace XTI_App.Abstractions;

public sealed class EmptyAppClientSessionKey : IAppClientSessionKey
{
    public string Value() => "";
}
