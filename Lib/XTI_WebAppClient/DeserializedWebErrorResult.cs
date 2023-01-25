using XTI_Core;

namespace XTI_WebAppClient;

internal sealed class DeserializedWebErrorResult
{
	public DeserializedWebErrorResult(string serialized)
	{
		if (string.IsNullOrWhiteSpace(serialized))
		{
            Value = new WebErrorResult();
		}
		else
        {
            var deserialized = XtiSerializer.Deserialize<ResultContainer<WebErrorResult>>(serialized).Data;
            if (string.IsNullOrWhiteSpace(deserialized?.LogEntryKey))
            {
                var errors = XtiSerializer.Deserialize<ResultContainer<ErrorModel[]>>(serialized).Data;
                deserialized = new WebErrorResult("", AppEventSeverity.Values.CriticalError, errors ?? new ErrorModel[0]);
            }
            Value = deserialized;
        }
	}

    public WebErrorResult Value { get; }
}
