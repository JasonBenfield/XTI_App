using XTI_WebApp.Abstractions;

namespace XTI_WebAppClient;

public sealed class GenericAppClientFactory
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessorFactory xtiTokenAccessorFactory;
    private readonly AppClientUrl clientUrl;
    private readonly IAppClientSessionKey sessionKey;
    private readonly IAppClientRequestKey requestKey;

    public GenericAppClientFactory
    (
        IHttpClientFactory httpClientFactory,
        XtiTokenAccessorFactory xtiTokenAccessorFactory,
        AppClientUrl clientUrl,
        IAppClientSessionKey sessionKey,
        IAppClientRequestKey requestKey
    )
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessorFactory = xtiTokenAccessorFactory;
        this.clientUrl = clientUrl;
        this.sessionKey = sessionKey;
        this.requestKey = requestKey;
    }

    public GenericAppClient Create(string appName) => Create(appName, "Current");

    public GenericAppClient Create(string appName, string versionKey) =>
        new
        (
            httpClientFactory,
            xtiTokenAccessorFactory,
            clientUrl,
            sessionKey,
            requestKey,
            appName,
            AppClientVersion.Version(versionKey)
        );

    public GenericAppClient Create(string appName, string versionKey, string domain) =>
        new
        (
            httpClientFactory,
            xtiTokenAccessorFactory,
            new AppClientUrl(new FixedAppClientDomain(domain)),
            sessionKey,
            requestKey,
            appName,
            AppClientVersion.Version(versionKey)
        );
}
