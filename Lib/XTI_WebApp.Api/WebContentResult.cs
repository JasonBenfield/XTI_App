namespace XTI_WebApp.Api;

public sealed class WebContentResult
{
    public WebContentResult(string content, string contentType = "text/plain")
    {
        Content = content;
        ContentType = contentType;
    }

    public string Content { get; }
    public string ContentType { get; }
}
