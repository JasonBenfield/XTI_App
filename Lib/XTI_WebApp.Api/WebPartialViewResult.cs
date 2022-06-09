namespace XTI_WebApp.Api;

public sealed class WebPartialViewResult
{
    public WebPartialViewResult(string viewName)
    {
        ViewName = viewName;
    }

    public string ViewName { get; }
}