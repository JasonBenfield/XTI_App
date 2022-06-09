using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class ViewAppAction<TModel> : AppAction<TModel, WebViewResult>
{
    private readonly string viewName;

    public ViewAppAction(string viewName)
    {
        this.viewName = viewName?.Trim() ?? "";
    }

    public Task<WebViewResult> Execute(TModel model, CancellationToken _ = default)
    {
        return Task.FromResult(new WebViewResult(viewName));
    }
}