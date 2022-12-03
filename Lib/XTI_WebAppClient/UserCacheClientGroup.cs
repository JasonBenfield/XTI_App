namespace XTI_WebAppClient;

public sealed class UserCacheClientGroup : AppClientGroup
{
    public UserCacheClientGroup(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options)
        : base(httpClientFactory, xtiTokenAccessor, clientUrl, options, "UserCache")
    {
    }

    public Task<EmptyActionResult> ClearCache(string userName, CancellationToken ct) =>
        CreatePostAction<string, EmptyActionResult>(nameof(ClearCache)).Post("", userName, ct);
}