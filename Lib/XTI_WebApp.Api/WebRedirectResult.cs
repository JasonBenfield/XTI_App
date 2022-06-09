namespace XTI_WebApp.Api;

public sealed class WebRedirectResult
{
    public WebRedirectResult(string url)
    {
        Url = url;
    }

    public string Url { get; }
}