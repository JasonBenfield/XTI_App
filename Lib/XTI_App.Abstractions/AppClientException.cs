using System.Text;
using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class AppClientException : Exception
{
    public AppClientException(string url, int statusCode, string responseContent, string logEntryKey, AppEventSeverity severity, ErrorModel[] errors)
        : base(FormatError(url, statusCode, responseContent, logEntryKey, severity, errors))
    {
        Url = url;
        StatusCode = statusCode;
        ResponseContent = responseContent;
        LogEntryKey = logEntryKey;
        Severity = severity;
        Errors = errors;
    }

    private static string FormatError(string url, int statusCode, string responseContent, string logEntryKey, AppEventSeverity severity, ErrorModel[] errors)
    {
        var joinedErrors = string.Join("\r\n", errors.Select(FormatError));
        return $"{url}\r\n{statusCode}\r\n{logEntryKey}\r\n{severity.DisplayText}\r\n{joinedErrors}\r\n{responseContent}";
    }

    private static string FormatError(ErrorModel error)
    {
        var formatted = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(error.Source))
        {
            formatted.Append($"[ {error.Source} ]");
        }
        if (!string.IsNullOrWhiteSpace(error.Caption))
        {
            if (formatted.Length > 0)
            {
                formatted.Append(" ");
            }
            formatted.Append(error.Caption);
        }
        if (!string.IsNullOrWhiteSpace(error.Message))
        {
            if (formatted.Length > 0)
            {
                formatted.Append(": ");
            }
            formatted.Append(error.Message);
        }
        return formatted.ToString();
    }

    public string Url { get; }
    public int StatusCode { get; }
    public string ResponseContent { get; }
    public string LogEntryKey { get; }
    public AppEventSeverity Severity { get; }
    public ErrorModel[] Errors { get; }

    public bool IsValidationFailed() => Severity.Equals(AppEventSeverity.Values.ValidationFailed);

    public ValidationFailedException ToValidationFailedException() =>
        new ValidationFailedException(Errors, this);
}