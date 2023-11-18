using XTI_WebApp.Abstractions;

namespace XTI_WebAppClient;

public sealed class GenericAppClientFactory
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private readonly IAppClientRequestKey requestKey;

    public GenericAppClientFactory
    (
        IHttpClientFactory httpClientFactory, 
        XtiTokenAccessor xtiTokenAccessor, 
        AppClientUrl clientUrl,
        IAppClientRequestKey requestKey
    )
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl;
        this.requestKey = requestKey;
    }

    public GenericAppClient Create(string appName) => Create(appName, "Current");

    public GenericAppClient Create(string appName, string versionKey) =>
        new GenericAppClient
        (
            httpClientFactory,
            xtiTokenAccessor,
            clientUrl,
            requestKey,
            appName,
            AppClientVersion.Version(versionKey)
        );

    public GenericAppClient Create(string appName, string versionKey, string domain) =>
        new GenericAppClient
        (
            httpClientFactory,
            xtiTokenAccessor,
            new AppClientUrl(new FixedAppClientDomain(domain)),
            requestKey,
            appName,
            AppClientVersion.Version(versionKey)
        );
}
