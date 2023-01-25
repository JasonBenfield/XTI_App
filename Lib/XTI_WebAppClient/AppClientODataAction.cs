using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using XTI_Core;

namespace XTI_WebAppClient;

public sealed class AppClientODataAction<TArgs, TEntity>
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private readonly AppClientOptions options;
    private readonly string actionName;

    public AppClientODataAction(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options, string actionName)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl;
        this.options = options;
        this.actionName = actionName;
    }

    public Task<string> Url() => Url("");

    public Task<string> Url(string modifier) => _Url(default, modifier);

    public Task<string> Url(TArgs model) => Url(model, "");

    public Task<string> Url(TArgs model, string modifier) => _Url(model, modifier);

    private async Task<string> _Url(TArgs? model, string modifier)
    {
        var url = await clientUrl.Value(actionName, modifier);
        var query = new ObjectToQueryString(model).Value;
        if (!string.IsNullOrWhiteSpace(query))
        {
            query = $"?{query}";
        }
        return $"{url}{query}";
    }

    public Task<ODataResult<TEntity>> Post(string odataOptions, TArgs model, CancellationToken ct) =>
        Post("", odataOptions, model, ct);

    public Task<ODataResult<TEntity>> Post(string modKey, string odataOptions, TArgs model, CancellationToken ct) =>
        _PostForQuery(modKey, odataOptions, model, true, ct);

    private async Task<ODataResult<TEntity>> _PostForQuery(string modKey, string odataOptions, TArgs model, bool retryUnauthorized, CancellationToken ct)
    {
        var query = new ObjectToQueryString(model).Value;
        var postResult = await GetPostResponse(modKey, query, odataOptions, ct);
        ODataResult<TEntity> odataResult;
        try
        {
            if (postResult.IsSuccessful)
            {
                odataResult = JsonSerializer.Deserialize<ODataResult<TEntity>>
                (
                    postResult.Content,
                    options.JsonSerializerOptions
                ) ?? throw new ArgumentNullException(nameof(odataResult));
            }
            else if (postResult.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
            {
                xtiTokenAccessor.Reset();
                odataResult = await _PostForQuery(modKey, odataOptions, model, false, ct);
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
        return odataResult;
    }

    private async Task<PostResult> GetPostResponse(string modKey, string query, string odataOptions, CancellationToken ct)
    {
        using var client = httpClientFactory.CreateClient();
        client.Timeout = options.Timeout;
        var token = await xtiTokenAccessor.Value();
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        var url = await clientUrl.ODataGet(modKey);
        if (!string.IsNullOrWhiteSpace(query))
        {
            url += query;
        }
        using var content = new StringContent(odataOptions, Encoding.UTF8, "text/plain");
        using var response = await client.PostAsync(url, content, ct);
        var responseContent = await response.Content.ReadAsStringAsync();
        return new PostResult(response.IsSuccessStatusCode, response.StatusCode, responseContent, url);
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
