using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class AccessDeniedAction : AppAction<EmptyRequest, WebViewResult>
{
    private readonly ViewAppAction<EmptyRequest> action = new ViewAppAction<EmptyRequest>("Index");

    public Task<WebViewResult> Execute(EmptyRequest model, CancellationToken stoppingToken) => action.Execute(model, stoppingToken);
}