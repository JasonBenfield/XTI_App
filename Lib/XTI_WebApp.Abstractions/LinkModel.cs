namespace XTI_WebApp.Abstractions;

public sealed record LinkModel(string LinkName, string DisplayText, string Url)
{
    public LinkModel()
        : this("", "", "") { }
}
