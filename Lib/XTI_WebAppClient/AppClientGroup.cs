using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace XTI_WebAppClient;

public class AppClientGroup
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private JsonSerializerOptions jsonSerializerOptions = new();

    protected AppClientGroup(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, string name)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl.WithGroup(name);
    }

    internal void SetJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
    {
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    protected Task<TResult> Post<TResult, TModel>(string action, string modifier, TModel model) =>
        _Post<TResult, TModel>(action, modifier, model, true);

    private async Task<TResult> _Post<TResult, TModel>(string action, string modifier, TModel model, bool retryUnauthorized)
    {
        var postResult = await GetPostResponse(action, modifier, model, "", "application/json");
        TResult result;
        try
        {
            if (postResult.IsSuccessful)
            {
                var resultContainer = JsonSerializer.Deserialize<ResultContainer<TResult>>
                (
                    postResult.Content, jsonSerializerOptions
                );
                if (resultContainer == null) { throw new ArgumentNullException(nameof(resultContainer)); }
                result = resultContainer.Data ?? throw new ArgumentNullException("resultContainer.Data");
            }
            else if (postResult.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
            {
                xtiTokenAccessor.Reset();
                result = await _Post<TResult, TModel>(action, modifier, model, false);
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

    protected Task<string> PostForContent<TModel>(string action, string modifier, TModel model)
        => _PostForContent(action, modifier, model, true);

    private async Task<string> _PostForContent<TModel>(string action, string modifier, TModel model, bool retryUnauthorized)
    {
        var postResult = await GetPostResponse(action, modifier, model, "", "text/plain");
        string result;
        try
        {
            if (postResult.IsSuccessful)
            {
                result = postResult.Content;
            }
            else if (postResult.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
            {
                xtiTokenAccessor.Reset();
                result = await _PostForContent(action, modifier, model, false);
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

    private async Task<PostResult> GetPostResponse(string action, string modifier, object? model, string query, string contentType)
    {
        using var client = httpClientFactory.CreateClient();
        if (!action.Equals("Authenticate", StringComparison.OrdinalIgnoreCase))
        {
            var token = await xtiTokenAccessor.Value();
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        var url = await clientUrl.Value(action, modifier);
        if (!string.IsNullOrWhiteSpace(query))
        {
            url += $"?{query}";
        }
        if (model == null) { throw new ArgumentNullException(nameof(model)); }
        object transformedModel = model;
        if (model is Forms.Form form)
        {
            transformedModel = form.Export();
        }
        var serialized = model is string modelString
            ? modelString
            : JsonSerializer.Serialize(model, jsonSerializerOptions);
        using var content = new StringContent(serialized, Encoding.UTF8, contentType);
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