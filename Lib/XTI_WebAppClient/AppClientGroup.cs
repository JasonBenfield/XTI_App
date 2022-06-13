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

    protected Task<TResult> Post<TResult, TModel>(string action, string modifier, TModel model)
        => Post<TResult, TModel>(action, modifier, model, "", false, "application/json", true);

    protected Task<string> PostForQuery(string action, string modifier, string odataOptions)
        => Post<string, string>(action, modifier, "", odataOptions, true, "application/json", true);

    protected Task<string> PostForContent<TModel>(string action, string modifier, TModel model)
        => Post<string, TModel>(action, modifier, model, "", true, "text/plain", true);

    private async Task<TResult> Post<TResult, TModel>(string action, string modifier, TModel model, string query, bool isContent, string contentType, bool retryUnauthorized)
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
        TResult result;
        try
        {
            if (response.IsSuccessStatusCode)
            {
                var resultContainer = JsonSerializer.Deserialize<ResultContainer<TResult>>
                (
                    responseContent, jsonSerializerOptions
                );
                if (resultContainer == null) { throw new ArgumentNullException(nameof(resultContainer)); }
                result = resultContainer.Data ?? throw new ArgumentNullException("resultContainer.Data");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
            {
                xtiTokenAccessor.Reset();
                result = await Post<TResult, TModel>(action, modifier, model, query, isContent, contentType, false);
            }
            else if (isContent)
            {
                result = (TResult)(object)responseContent;
            }
            else
            {
                var resultContainer = string.IsNullOrWhiteSpace(responseContent)
                    ? new ResultContainer<ErrorModel[]> { Data = new ErrorModel[0] }
                    : JsonSerializer.Deserialize<ResultContainer<ErrorModel[]>>(responseContent);
                throw new AppClientException
                (
                    url,
                    response.StatusCode,
                    responseContent,
                    resultContainer?.Data ?? new ErrorModel[0]
                );
            }
        }
        catch (JsonException ex)
        {
            throw new AppClientException
            (
                url,
                response.StatusCode,
                responseContent,
                new[] { new ErrorModel { Message = ex.Message } }
            );
        }
        return result;
    }
}