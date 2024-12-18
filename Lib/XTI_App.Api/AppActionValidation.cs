using XTI_Core;

namespace XTI_App.Api;

public interface AppActionValidation<TModel>
{
    Task Validate(ErrorList errors, TModel requestData, CancellationToken stoppingToken);
}