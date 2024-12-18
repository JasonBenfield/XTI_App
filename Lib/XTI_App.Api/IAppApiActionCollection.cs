namespace XTI_App.Api;

public interface IAppApiActionCollection
{
    IEnumerable<IAppApiAction> Actions();
    AppApiAction<TModel, TResult> Action<TModel, TResult>(string actionName);
    AppApiAction<TRequest, TResult> Add<TRequest, TResult>(AppApiAction<TRequest, TResult> action);
}