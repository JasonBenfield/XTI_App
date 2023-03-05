namespace XTI_App.Abstractions;

public sealed record WebContentResult(string Content, string ContentType = "text/plain")
{
    public WebContentResult()
        : this("", "")
    {
    }
}
