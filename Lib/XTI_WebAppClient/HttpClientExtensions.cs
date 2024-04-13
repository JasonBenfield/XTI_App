using XTI_WebApp.Abstractions;

namespace XTI_WebAppClient;

internal static class HttpClientExtensions
{
    internal static void InitFromOptions(this HttpClient client, AppClientOptions options)
    {
        client.Timeout = options.Timeout;
        var requestKey = options.RequestKey.Value();
        if (!string.IsNullOrWhiteSpace(requestKey))
        {
            client.DefaultRequestHeaders.Add(new SourceRequestKeyHeader().Value, requestKey);
        }
    }
}
