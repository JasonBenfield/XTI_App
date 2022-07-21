using System.Text.Json;
using System.Text.Json.Serialization;

namespace XTI_WebAppClient;

public class AppClient
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly AppClientUrl clientUrl;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientOptions options = new();

    protected AppClient(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, string appName, string version)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl.WithApp(appName, version);
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        jsonSerializerOptions.PropertyNamingPolicy = null;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions = jsonSerializerOptions;
    }

    protected T CreateGroup<T>(Func<IHttpClientFactory, XtiTokenAccessor, AppClientUrl, AppClientOptions, T> createGroup)
        where T : AppClientGroup
    {
        var group = createGroup
        (
            httpClientFactory,
            xtiTokenAccessor,
            clientUrl,
            options
        );
        return group;
    }

    protected AppClientODataGroup<TArgs, TEntity> CreateODataGroup<TArgs, TEntity>(string groupName)
    {
        var odataGroup = new AppClientODataGroup<TArgs, TEntity>
        (
            httpClientFactory,
            xtiTokenAccessor,
            clientUrl,
            options,
            groupName
        );
        return odataGroup;
    }

    protected virtual void ConfigureJsonSerializerOptions(JsonSerializerOptions options)
    {
    }

    public Task<string> UserName() => xtiTokenAccessor.UserName();

    public void UseToken<T>() where T : IXtiToken => xtiTokenAccessor.UseToken<T>();

    public override string ToString() => $"{GetType().Name}";
}