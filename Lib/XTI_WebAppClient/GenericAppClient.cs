namespace XTI_WebAppClient;

public sealed class GenericAppClient : AppClient
{
    public GenericAppClient
    (
        IHttpClientFactory httpClientFactory,
        XtiTokenAccessorFactory xtiTokenAccessorFactory,
        AppClientUrl clientUrl,
        AppClientOptions options,
        string appName,
        AppClientVersion version
    )
        : base
        (
            httpClientFactory,
            xtiTokenAccessorFactory,
            clientUrl,
            options,
            appName,
            version.IsBlank() ? "Current" : version.Value
        )
    {
    }

    public GenericAppClientGroup Group(string name) =>
        CreateGroup
        (
            (_httpClientFactory, _tokenAccessor, _clientUrl, _options) =>
                new GenericAppClientGroup(_httpClientFactory, _tokenAccessor, _clientUrl, _options, name)
        );

}