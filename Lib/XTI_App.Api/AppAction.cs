namespace XTI_App.Api;

public interface AppAction<TModel, TResult>
{
    Task<TResult> Execute(TModel requestData, CancellationToken stoppingToken);
}