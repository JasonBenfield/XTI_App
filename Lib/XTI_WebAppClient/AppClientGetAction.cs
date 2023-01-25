using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.Arm;
using XTI_Core;
using XTI_Forms;

namespace XTI_WebAppClient;

public sealed class AppClientGetAction<TModel>
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private readonly AppClientOptions options;
    private readonly string actionName;

    public AppClientGetAction(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options, string actionName)
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

    public Task<string> GetContent(string modifier, TModel model, CancellationToken ct)
        => _GetContent(modifier, model, true, ct);

    private async Task<string> _GetContent(string modifier, TModel model, bool retryUnauthorized, CancellationToken ct)
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
        var serialized = transformedModel is string modelString
            ? modelString
            : new ObjectToQueryString(transformedModel).Value;
        if (!string.IsNullOrWhiteSpace(serialized))
        {
            if (!serialized.StartsWith("?"))
            {
                url += "?";
            }
            url += serialized;
        }
        var response = await client.GetAsync(url, ct);
        string result;
        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadAsStringAsync();
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
        {
            xtiTokenAccessor.Reset();
            result = await _GetContent(modifier, model, false, ct);
        }
        else
        {
            throw new AppClientException
            (
                response.RequestMessage?.RequestUri?.ToString() ?? "",
                (int)response.StatusCode,
                "",
                "",
                AppEventSeverity.Values.CriticalError,
                new ErrorModel[0]
            );
        }
        return result;
    }

}
