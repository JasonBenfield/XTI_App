namespace XTI_WebApp.Api;

public sealed class WebViewResult
{
    public WebViewResult(string viewName)
    {
        ViewName = viewName;
    }

    public string ViewName { get; }
}