using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class WebViewResultFactory
{
    private readonly IPageContext pageContext;

    public WebViewResultFactory(IPageContext pageContext)
    {
        this.pageContext = pageContext;
    }

    public WebViewResult Default(string pageName, string title) =>
        Create("Default", pageName, title);

    public WebViewResult Create(string viewName, string title) =>
        Create(viewName, "", title);

    private WebViewResult Create(string viewName, string pageName, string title)
    {
        pageContext.PageTitle = title;
        pageContext.PageName = pageName;
        return new WebViewResult(viewName);
    }
}
