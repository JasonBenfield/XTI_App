namespace XTI_App.Api;

public static class AppApiGroupExtensions
{
    public static AppApiAction<TModel, TResult> Action<TModel, TResult>(this IAppApiGroup group, string actionName) =>
        group.Action<AppApiAction<TModel, TResult>>(actionName);

}
