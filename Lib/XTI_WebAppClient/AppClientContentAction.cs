using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using XTI_Core;
using XTI_Forms;

namespace XTI_WebAppClient;

public sealed class AppClientContentAction<TRequest>
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private readonly AppClientOptions options;
    private readonly string actionName;

    public AppClientContentAction(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options, string actionName)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl;
        this.options = options;
        this.actionName = actionName;
    }

    public Task<string> Url() => Url("");

    public Task<string> Url(string modifier) => _Url(default, modifier);

    public Task<string> Url(TRequest requestData) => Url(requestData, "");

    public Task<string> Url(TRequest requestData, string modifier) => _Url(requestData, modifier);

    private async Task<string> _Url(TRequest? requestData, string modifier)
    {
        var url = await clientUrl.Value(actionName, modifier);
        var query = new ObjectToQueryString(requestData).Value;
        if (!string.IsNullOrWhiteSpace(query))
        {
            query = $"?{query}";
        }
        return $"{url}{query}";
    }

    public Task<WebContentResult> Post(string modifier, TRequest requestData, CancellationToken ct) =>
        _Post(modifier, requestData, true, ct);

    private async Task<WebContentResult> _Post(string modifier, TRequest requestData, bool retryUnauthorized, CancellationToken ct)
    {
        using var response = await GetPostResponseMessage(modifier, requestData, ct);
        var responseContent = await response.Content.ReadAsStringAsync();
        WebContentResult result;
        try
        {
            if (response.IsSuccessStatusCode)
            {
                result = new WebContentResult(responseContent, response.Content.Headers.ContentType?.MediaType ?? "");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
            {
                xtiTokenAccessor.Reset();
                result = await _Post(modifier, requestData, false, ct);
            }
            else
            {
                throw CreatePostException(response, responseContent);
            }
        }
        catch (JsonException ex)
        {
            throw CreateJsonException(response, responseContent, ex);
        }
        return result;
    }

    private async Task<HttpResponseMessage> GetPostResponseMessage(string modifier, object? requestData, CancellationToken ct)
    {
        using var client = httpClientFactory.CreateClient();
        client.InitFromOptions(options);
        if (!actionName.Equals("Authenticate", StringComparison.OrdinalIgnoreCase))
        {
            var token = await xtiTokenAccessor.Value(ct);
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        var url = await clientUrl.Value(actionName, modifier);
        if (requestData == null) { throw new ArgumentNullException(nameof(requestData)); }
        object transformedModel = requestData;
        if (requestData is Form form)
        {
            transformedModel = form.Export();
        }
        var serialized = JsonSerializer.Serialize(transformedModel, options.JsonSerializerOptions);
        using var content = new StringContent(serialized, Encoding.UTF8, WebContentTypes.Json);
        var response = await client.PostAsync(url, content, ct);
        return response;
    }

    private static AppClientException CreatePostException(HttpResponseMessage response, string content)
    {
        var errorResult = new DeserializedWebErrorResult(content).Value;
        var ex = new AppClientException
        (
            response.RequestMessage?.RequestUri?.ToString() ?? "",
            (int)response.StatusCode,
            content,
            errorResult.LogEntryKey,
            errorResult.Severity,
            errorResult.Errors
        );
        return ex;
    }

    private static AppClientException CreateJsonException(HttpResponseMessage response, string content, JsonException ex) =>
        new AppClientException
        (
            response.RequestMessage?.RequestUri?.ToString() ?? "",
            (int)response.StatusCode,
            content,
            "",
            AppEventSeverity.Values.CriticalError,
            [new ErrorModel(ex.Message)]
        );
}
