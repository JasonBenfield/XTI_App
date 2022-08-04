using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace XTI_WebAppClient;

public sealed class AppClientODataToExcelAction<TArgs, TEntity>
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private readonly string actionName;

    public AppClientODataToExcelAction(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, string actionName)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl;
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
        return $"{url}{query}";
    }

    public Task<AppClientFileResult> GetFile(string odataOptions, TArgs model) =>
        GetFile("", odataOptions, model);

    public Task<AppClientFileResult> GetFile(string modKey, string odataOptions, TArgs model) =>
        _GetFile(modKey, odataOptions, model, true);

    private async Task<AppClientFileResult> _GetFile(string modKey, string odataOptions, TArgs model, bool retryUnauthorized)
    {
        var query = "";
        if (!string.IsNullOrWhiteSpace(odataOptions))
        {
            query += $"?{odataOptions}";
        }
        var modelAsQuery = new ObjectToQueryString(model).Value;
        if (!string.IsNullOrWhiteSpace(modelAsQuery))
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                query += "?";
            }
            else
            {
                query += "&";
            }
            query += modelAsQuery.Substring(1);
        }
        var postResult = await GetPostResponse(modKey, query);
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
                fileResult = await _GetFile(modKey, odataOptions, model, false);
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

    private async Task<PostResult> GetPostResponse(string modKey, string query)
    {
        using var client = httpClientFactory.CreateClient();
        var token = await xtiTokenAccessor.Value();
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        var url = await clientUrl.Value(actionName, modKey);
        if (!string.IsNullOrWhiteSpace(query))
        {
            url += query;
        }
        using var response = await client.GetAsync(url);
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
            postResult.StatusCode,
            "",
            new ErrorModel[0]
        );
        return ex;
    }

    private static AppClientException CreateJsonException(PostResult postResult, JsonException ex) =>
        new AppClientException
        (
            postResult.Url,
            postResult.StatusCode,
            "",
            new[] { new ErrorModel { Message = ex.Message } }
        );

    private sealed record PostResult(bool IsSuccessful, HttpStatusCode StatusCode, byte[] Content, string ContentType, string FileName, string Url);
}
