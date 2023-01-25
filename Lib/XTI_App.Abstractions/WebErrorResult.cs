using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class WebErrorResult
{
    public WebErrorResult()
        : this("", AppEventSeverity.Values.GetDefault(), new ErrorModel[0])
    {
    }

    public WebErrorResult(string logEntryKey, AppEventSeverity severity, ErrorModel[] errors)
    {
        LogEntryKey = logEntryKey;
        Severity = severity;
        Errors = errors;
    }

    public string LogEntryKey { get; }
    public AppEventSeverity Severity { get; }
    public ErrorModel[] Errors { get; }
}
