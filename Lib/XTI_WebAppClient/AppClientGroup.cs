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
        => Post<TResult, TModel>(action, modifier, model, true);

    private async Task<TResult> Post<TResult, TModel>(string action, string modifier, TModel model, bool retryUnauthorized)
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
        if (model == null) { throw new ArgumentNullException(nameof(model)); }
        object transformedModel = model;
        if (model is Forms.Form form)
        {
            transformedModel = form.Export();
        }
        var serialized = JsonSerializer.Serialize(model, jsonSerializerOptions);
        using var content = new StringContent(serialized, Encoding.UTF8, "application/json");
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
                result = await Post<TResult, TModel>(action, modifier, model, false);
            }
            else
            {
                var resultContainer = string.IsNullOrWhiteSpace(responseContent)
                    ? new ResultContainer<ErrorModel[]>() { Data = new ErrorModel[0] }
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