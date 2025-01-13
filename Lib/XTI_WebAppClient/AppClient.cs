namespace XTI_WebAppClient;

public class AppClient
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly AppClientUrl clientUrl;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientOptions options;

    protected AppClient
    (
        IHttpClientFactory httpClientFactory,
        XtiTokenAccessorFactory xtiTokenAccessorFactory,
        AppClientUrl clientUrl,
        IAppClientSessionKey sessionKey,
        IAppClientRequestKey requestKey,
        string appName,
        string version
    )
    {
        this.httpClientFactory = httpClientFactory;
        xtiTokenAccessor = xtiTokenAccessorFactory.Create();
        this.clientUrl = clientUrl.WithApp(appName, version);
        options = new AppClientOptions(sessionKey, requestKey);
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

    public UserCacheClientGroup UserCache { get; }

    public UserClientGroup User { get; }

    public Task<string> UserName() => xtiTokenAccessor.UserName();

    public void ConfigureOptions(Action<AppClientOptions> configure)
    {
        configure(options);
    }

    public void UseToken<T>() where T : IXtiToken => xtiTokenAccessor.UseToken<T>();

    public void SetTimeOut(TimeSpan timeOut)
    {
        options.Timeout = timeOut;
    }

    public override string ToString() => $"{GetType().Name}";
}