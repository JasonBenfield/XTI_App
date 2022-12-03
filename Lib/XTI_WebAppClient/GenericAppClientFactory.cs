using XTI_WebApp.Abstractions;

namespace XTI_WebAppClient;

public sealed class GenericAppClientFactory
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;

    public GenericAppClientFactory(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl;
    }

    public GenericAppClient Create(string appName) => Create(appName, "Current");

    public GenericAppClient Create(string appName, string versionKey) =>
        new GenericAppClient
        (
            httpClientFactory,
            xtiTokenAccessor,
            clientUrl,
            appName,
            AppClientVersion.Version(versionKey)
        );

    public GenericAppClient Create(string appName, string versionKey, string domain) =>
        new GenericAppClient
        (
            httpClientFactory,
            xtiTokenAccessor,
            new AppClientUrl(new FixedAppClientDomain(domain)),
            appName,
            AppClientVersion.Version(versionKey)
        );
}
