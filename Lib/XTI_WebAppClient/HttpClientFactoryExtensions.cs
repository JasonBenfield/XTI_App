using XTI_WebApp.Abstractions;

namespace XTI_WebAppClient;

internal static class HttpClientFactoryExtensions
{
    internal static void InitFromOptions(this HttpClient client, AppClientOptions options)
    {
        client.Timeout = options.Timeout;
        if (!string.IsNullOrWhiteSpace(options.SourceSessionKey))
        {
            client.DefaultRequestHeaders.Add(new SourceRequestKeyHeader().Value, options.SourceSessionKey);
        }
    }
}
