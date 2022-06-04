using XTI_Core;

namespace XTI_App.Api;

public sealed class AppActionValidationNotRequired<TModel> : AppActionValidation<TModel>
{
    public Task Validate(ErrorList errors, TModel model, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}