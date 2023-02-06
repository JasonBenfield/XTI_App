using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using XTI_Core;

namespace XTI_WebAppClient;

public sealed class AppClientODataToExcelAction<TArgs, TEntity>
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private readonly AppClientOptions options;
    private readonly string actionName;

    public AppClientODataToExcelAction(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options, string actionName)
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

    public Task<AppClientFileResult> GetFile(string odataOptions, TArgs model, CancellationToken ct) =>
        GetFile("", odataOptions, model, ct);

    public Task<AppClientFileResult> GetFile(string modKey, string odataOptions, TArgs model, CancellationToken ct) =>
        _GetFile(modKey, odataOptions, model, true, ct);

    private async Task<AppClientFileResult> _GetFile(string modKey, string odataOptions, TArgs model, bool retryUnauthorized, CancellationToken ct)
    {
        var query = new ObjectToQueryString(model).Value;
        var postResult = await GetPostResponse(modKey, query, odataOptions, ct);
        AppClientFileResult fileResult;
        try
        {
            if (postResult.IsSuccessful)
            {
                fileResult = new AppClientFileResult
                (
                    postResult.Content,
                    postResult.ContentType,
                    postResult.FileName
                );
            }
            else if (postResult.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
            {
                xtiTokenAccessor.Reset();
                fileResult = await _GetFile(modKey, odataOptions, model, false, ct);
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
        return fileResult;
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
        var url = await clientUrl.Value(actionName, modKey);
        if (!string.IsNullOrWhiteSpace(query))
        {
            url += $"?{query}";
        }
        if (!string.IsNullOrWhiteSpace(odataOptions))
        {
            url += string.IsNullOrWhiteSpace(query) ? "?" : "&";
            url += odataOptions;
        }
        using var response = await client.GetAsync(url, ct);
        var responseContent = await response.Content.ReadAsByteArrayAsync();
        return new PostResult
        (
            response.IsSuccessStatusCode,
            response.StatusCode,
            responseContent,
            response.Content.Headers.ContentType?.MediaType ?? "",
            response.Content.Headers.ContentDisposition?.FileName ?? "",
            url
        );
    }

    private static AppClientException CreatePostException(PostResult postResult)
    {
        var ex = new AppClientException
        (
            postResult.Url,
            (int)postResult.StatusCode,
            "",
            "",
            AppEventSeverity.Values.CriticalError,
            new ErrorModel[0]
        );
        return ex;
    }

    private static AppClientException CreateJsonException(PostResult postResult, JsonException ex) =>
        new AppClientException
        (
            postResult.Url,
            (int)postResult.StatusCode,
            "",
            "",
            AppEventSeverity.Values.CriticalError,
            new[] { new ErrorModel(ex.Message) }
        );

    private sealed record PostResult(bool IsSuccessful, HttpStatusCode StatusCode, byte[] Content, string ContentType, string FileName, string Url);
}
