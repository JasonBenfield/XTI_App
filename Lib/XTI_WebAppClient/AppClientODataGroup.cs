using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace XTI_WebAppClient;

public sealed class AppClientODataGroup<TEntity>
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private JsonSerializerOptions jsonSerializerOptions = new();

    public AppClientODataGroup(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, string name)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl.WithGroup(name);
    }

    internal void SetJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
    {
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    public Task<ODataResult<TEntity>> Get(string odataOptions) =>
        _PostForQuery(odataOptions, true);

    private async Task<ODataResult<TEntity>> _PostForQuery(string odataOptions, bool retryUnauthorized)
    {
        var postResult = await GetPostResponse(odataOptions);
        ODataResult<TEntity> odataResult;
        try
        {
            if (postResult.IsSuccessful)
            {
                odataResult = JsonSerializer.Deserialize<ODataResult<TEntity>>
                (
                    postResult.Content, jsonSerializerOptions
                ) ?? throw new ArgumentNullException(nameof(odataResult));
            }
            else if (postResult.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
            {
                xtiTokenAccessor.Reset();
                odataResult = await _PostForQuery(odataOptions, false);
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

    private async Task<PostResult> GetPostResponse(string query)
    {
        using var client = httpClientFactory.CreateClient();
        var token = await xtiTokenAccessor.Value();
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        var url = await clientUrl.Value("", "");
        if (!string.IsNullOrWhiteSpace(query))
        {
            url += $"?{query}";
        }
        using var content = new StringContent("", Encoding.UTF8, "text/plain");
        using var response = await client.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return new PostResult(response.IsSuccessStatusCode, response.StatusCode, responseContent, url);
    }

    private static AppClientException CreatePostException(PostResult postResult)
    {
        var resultContainer = string.IsNullOrWhiteSpace(postResult.Content)
            ? new ResultContainer<ErrorModel[]> { Data = new ErrorModel[0] }
            : JsonSerializer.Deserialize<ResultContainer<ErrorModel[]>>(postResult.Content);
        var ex = new AppClientException
        (
            postResult.Url,
            postResult.StatusCode,
            postResult.Content,
            resultContainer?.Data ?? new ErrorModel[0]
        );
        return ex;
    }

    private static AppClientException CreateJsonException(PostResult postResult, JsonException ex) =>
        new AppClientException
        (
            postResult.Url,
            postResult.StatusCode,
            postResult.Content,
            new[] { new ErrorModel { Message = ex.Message } }
        );

    private sealed record PostResult(bool IsSuccessful, HttpStatusCode StatusCode, string Content, string Url);
}