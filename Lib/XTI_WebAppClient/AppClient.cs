using System.Text.Json;
using System.Text.Json.Serialization;
using XTI_Core;

namespace XTI_WebAppClient;

public class AppClient
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly AppClientUrl clientUrl;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientOptions options = new();

    protected AppClient
    (
        IHttpClientFactory httpClientFactory, 
        XtiTokenAccessorFactory xtiTokenAccessorFactory, 
        AppClientUrl clientUrl, 
        IAppClientRequestKey requestKey, 
        string appName, 
        string version
    )
    {
        this.httpClientFactory = httpClientFactory;
        xtiTokenAccessor = xtiTokenAccessorFactory.Create();
        this.clientUrl = clientUrl.WithApp(appName, version);
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        jsonSerializerOptions.PropertyNamingPolicy = null;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        jsonSerializerOptions.AddCoreConverters();
        options.JsonSerializerOptions = jsonSerializerOptions;
        ConfigureJsonSerializerOptions(jsonSerializerOptions);
        options.RequestKey = requestKey;
        UserCache = CreateGroup
        (
            (_httpClientFactory, _tokenAccessor, _clientUrl, _options) =>
                new UserCacheClientGroup(_httpClientFactory, _tokenAccessor, _clientUrl, _options)
        );
        User = CreateGroup
        (
            (_httpClientFactory, _tokenAccessor, _clientUrl, _options) =>
                new UserClientGroup(_httpClientFactory, _tokenAccessor, _clientUrl, _options)
        );
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

    public UserCacheClientGroup UserCache { get; }

    public UserClientGroup User { get; }

    public Task<string> UserName() => xtiTokenAccessor.UserName();

    public void UseToken<T>() where T : IXtiToken => xtiTokenAccessor.UseToken<T>();

    public void SetTimeOut(TimeSpan timeOut)
    {
        options.Timeout = timeOut;
    }

    public override string ToString() => $"{GetType().Name}";
}