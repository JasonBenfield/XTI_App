namespace XTI_App.Abstractions;

public sealed record WebContentResult(string Content, string ContentType = WebContentTypes.Text)
{
    public WebContentResult()
        : this("", "")
    {
    }
}
