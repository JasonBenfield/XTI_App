using XTI_Core;

namespace XTI_App.Api;

public sealed class AppActionValidationNotRequired<TModel> : AppActionValidation<TModel>
{
    public Task Validate(ErrorList errors, TModel model)
    {
        return Task.CompletedTask;
    }
}