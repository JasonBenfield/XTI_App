using XTI_Core;

namespace XTI_WebAppClient;

public sealed class DeserializedWebErrorResult
{
    public DeserializedWebErrorResult(string serialized)
    {
        if (string.IsNullOrWhiteSpace(serialized))
        {
            Value = new WebErrorResult();
        }
        else if (serialized.Contains("\"LogEntryKey\":") && serialized.Contains("\"Severity\":"))
        {
            Value = XtiSerializer.Deserialize<ResultContainer<WebErrorResult>>(serialized).Data ?? new WebErrorResult();
        }
        else
        {
            var errors = XtiSerializer.Deserialize<ResultContainer<ErrorModel[]>>(serialized).Data;
            Value = new WebErrorResult("", AppEventSeverity.Values.CriticalError, errors ?? new ErrorModel[0]);
        }
    }

    public WebErrorResult Value { get; }
}
