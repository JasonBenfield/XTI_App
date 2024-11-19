namespace XTI_WebApp.Abstractions;

public sealed record ResourcePath(string Group, string Action, string ModKey)
{
    public ResourcePath()
        : this("", "", "")
    {
    }
}
