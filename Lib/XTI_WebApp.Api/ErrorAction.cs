using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class ErrorAction : AppAction<EmptyRequest, WebViewResult>
{
    private readonly ViewAppAction<EmptyRequest> action = new ViewAppAction<EmptyRequest>("Error");

    public Task<WebViewResult> Execute(EmptyRequest model, CancellationToken stoppingToken) => action.Execute(model, stoppingToken);
}