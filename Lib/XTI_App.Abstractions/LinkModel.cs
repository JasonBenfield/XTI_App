namespace XTI_App.Abstractions;

public sealed record LinkModel(string LinkName, string DisplayText, string Url, bool IsAuthenticationRequired = true)
{
    public LinkModel()
        : this("", "", "", true) { }
}
