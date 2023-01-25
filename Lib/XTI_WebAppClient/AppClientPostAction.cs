using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_Forms;

namespace XTI_WebAppClient;

public sealed class AppClientPostAction<TModel, TResult>
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private readonly AppClientOptions options;
    private readonly string actionName;

    public AppClientPostAction(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options, string actionName)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl;
        this.options = options;
        this.actionName = actionName;
    }

    public Task<string> Url() => Url("");

    public Task<string> Url(string modifier) => _Url(default, modifier);

    public Task<string> Url(TModel model) => Url(model, "");

    public Task<string> Url(TModel model, string modifier) => _Url(model, modifier);

    private async Task<string> _Url(TModel? model, string modifier)
    {
        var url = await clientUrl.Value(actionName, modifier);
        var query = new ObjectToQueryString(model).Value;
        if (!string.IsNullOrWhiteSpace(query))
        {
            query = $"?{query}";
        }
        return $"{url}{query}";
    }

    public Task<TResult> Post(string modifier, TModel model, CancellationToken ct) =>
        _Post(modifier, model, true, ct);

    private async Task<TResult> _Post(string modifier, TModel model, bool retryUnauthorized, CancellationToken ct)
    {
        var postResult = await GetPostResponse(modifier, model, ct);
        TResult result;
        try
        {
            if (postResult.IsSuccessful)
            {
                var resultContainer = JsonSerializer.Deserialize<ResultContainer<TResult>>
                (
                    postResult.Content, options.JsonSerializerOptions
                );
                if (resultContainer == null) { throw new ArgumentNullException(nameof(resultContainer)); }
                result = resultContainer.Data ?? throw new ArgumentNullException("resultContainer.Data");
            }
            else if (postResult.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
            {
                xtiTokenAccessor.Reset();
                result = await _Post(modifier, model, false, ct);
            }
            else
            {
                throw CreatePostException(postResult);
            }
        }
        catch (JsonException ex)
        {
            throw CreateJsonException(postResult, ex);
        }
        return result;
    }

    private async Task<PostResult> GetPostResponse(string modifier, object? model, CancellationToken ct)
    {
        PostResult postResult;
        var response = await GetPostResponseMessage(modifier, model, ct);
        try
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            postResult = new PostResult
            (
                response.IsSuccessStatusCode,
                response.StatusCode,
                responseContent,
                response.RequestMessage?.RequestUri?.ToString() ?? ""
            );
        }
        finally
        {
            response.Dispose();
        }
        return postResult;
    }

    private async Task<HttpResponseMessage> GetPostResponseMessage(string modifier, object? model, CancellationToken ct)
    {
        using var client = httpClientFactory.CreateClient();
        client.Timeout = options.Timeout;
        if (!actionName.Equals("Authenticate", StringComparison.OrdinalIgnoreCase))
        {
            var token = await xtiTokenAccessor.Value();
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        var url = await clientUrl.Value(actionName, modifier);
        if (model == null) { throw new ArgumentNullException(nameof(model)); }
        object transformedModel = model;
        if (model is Form form)
        {
            transformedModel = form.Export();
        }
        var serialized = JsonSerializer.Serialize(transformedModel, options.JsonSerializerOptions);
        using var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content, ct);
        return response;
    }

    private static AppClientException CreatePostException(PostResult postResult)
    {
        var errorResult = new DeserializedWebErrorResult(postResult.Content).Value;
        var ex = new AppClientException
        (
            postResult.Url,
            (int)postResult.StatusCode,
            postResult.Content,
            errorResult.LogEntryKey,
            errorResult.Severity,
            errorResult.Errors
        );
        return ex;
    }

    private static AppClientException CreateJsonException(PostResult postResult, JsonException ex) =>
        new AppClientException
        (
            postResult.Url,
            (int)postResult.StatusCode,
            postResult.Content,
            "",
            AppEventSeverity.Values.CriticalError,
            new[] { new ErrorModel(ex.Message) }
        );

    private sealed record PostResult(bool IsSuccessful, HttpStatusCode StatusCode, string Content, string Url);
}
