namespace XTI_WebAppClient;

public sealed class GenericAppClient : AppClient
{
    public GenericAppClient(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, string appName, AppClientVersion version)
        : base(httpClientFactory, xtiTokenAccessor, clientUrl, appName, version.IsBlank() ? "Current" : version.Value)
    {
        UserCache = CreateGroup
        (
            (_httpClientFactory, _tokenAccessor, _clientUrl, _options) =>
                new UserCacheClientGroup(_httpClientFactory, _tokenAccessor, _clientUrl, _options)
        );
    }

    public UserCacheClientGroup UserCache { get; }

    public GenericAppClientGroup Group(string name) =>
        CreateGroup
        (
            (_httpClientFactory, _tokenAccessor, _clientUrl, _options) =>
                new GenericAppClientGroup(_httpClientFactory, _tokenAccessor, _clientUrl, _options, name)
        );

}