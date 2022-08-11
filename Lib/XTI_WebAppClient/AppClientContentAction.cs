using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace XTI_WebAppClient;

public sealed class AppClientContentAction<TModel>
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

    public Task<AppClientContentResult> Post(string modifier, TModel model) =>
        _Post(modifier, model, true);

    private async Task<AppClientContentResult> _Post(string modifier, TModel model, bool retryUnauthorized)
    {
        using var response = await GetPostResponseMessage(modifier, model);
        var responseContent = await response.Content.ReadAsStringAsync();
        AppClientContentResult result;
        try
        {
            if (response.IsSuccessStatusCode)
            {
                result = new AppClientContentResult(responseContent, response.Content.Headers.ContentType?.MediaType ?? "");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
            {
                xtiTokenAccessor.Reset();
                result = await _Post(modifier, model, false);
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

    private async Task<HttpResponseMessage> GetPostResponseMessage(string modifier, object? model)
    {
        using var client = httpClientFactory.CreateClient();
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
        if (model is Forms.Form form)
        {
            transformedModel = form.Export();
        }
        var serialized = transformedModel is string modelString
            ? modelString
            : JsonSerializer.Serialize(transformedModel, options.JsonSerializerOptions);
        using var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        return response;
    }

    private static AppClientException CreatePostException(HttpResponseMessage response, string content)
    {
        var resultContainer = string.IsNullOrWhiteSpace(content)
            ? new ResultContainer<ErrorModel[]> { Data = new ErrorModel[0] }
            : JsonSerializer.Deserialize<ResultContainer<ErrorModel[]>>(content);
        var ex = new AppClientException
        (
            response.RequestMessage?.RequestUri?.ToString() ?? "",
            response.StatusCode,
            content,
            resultContainer?.Data ?? new ErrorModel[0]
        );
        return ex;
    }

    private static AppClientException CreateJsonException(HttpResponseMessage response, string content, JsonException ex) =>
        new AppClientException
        (
            response.RequestMessage?.RequestUri?.ToString() ?? "",
            response.StatusCode,
            content,
            new[] { new ErrorModel { Message = ex.Message } }
        );
}
