using System.Text.Json;
using System.Text.Json.Serialization;

namespace XTI_WebAppClient;

public class AppClient
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly AppClientUrl clientUrl;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();

    protected AppClient(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, string appName, string version)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl.WithApp(appName, version);
        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        jsonSerializerOptions.PropertyNamingPolicy = null;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        ConfigureJsonSerializerOptions(jsonSerializerOptions);
    }

    protected T GetGroup<T>(Func<IHttpClientFactory, XtiTokenAccessor, AppClientUrl, T> createGroup)
        where T : AppClientGroup
    {
        var group = createGroup
        (
            httpClientFactory,
            xtiTokenAccessor,
            clientUrl
        );
        group.SetJsonSerializerOptions(jsonSerializerOptions);
        return group;
    }

    protected virtual void ConfigureJsonSerializerOptions(JsonSerializerOptions options)
    {
    }

    public Task<string> UserName() => xtiTokenAccessor.UserName();

    public void UseToken<T>() where T : IXtiToken => xtiTokenAccessor.UseToken<T>();

    public override string ToString() => $"{GetType().Name}";
}