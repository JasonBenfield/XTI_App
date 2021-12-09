namespace XTI_App.Api;

public interface OptionalAction<TModel, TResult> : AppAction<TModel, TResult>
{
    Task<bool> IsOptional();
}