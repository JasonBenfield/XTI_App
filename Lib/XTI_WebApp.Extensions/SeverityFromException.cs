using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_WebApp.Extensions;

internal sealed class SeverityFromException
{
	public SeverityFromException(Exception ex)
    {
        if (ex is ValidationFailedException)
        {
            Value = AppEventSeverity.Values.ValidationFailed;
        }
        else if (ex is AccessDeniedException)
        {
            Value = AppEventSeverity.Values.AccessDenied;
        }
        else if (ex is AppException)
        {
            Value = AppEventSeverity.Values.AppError;
        }
        else
        {
            Value = AppEventSeverity.Values.CriticalError;
        }
    }

    public AppEventSeverity Value { get; }
}
