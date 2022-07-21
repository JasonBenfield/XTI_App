namespace XTI_WebAppClient;

public sealed class UserCacheClientGroup : AppClientGroup
{
    public UserCacheClientGroup(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options)
        : base(httpClientFactory, xtiTokenAccessor, clientUrl, options, "UserCache")
    {
    }

    public Task<EmptyActionResult> ClearCache(ClearUserCacheRequest request) =>
        CreatePostAction<ClearUserCacheRequest, EmptyActionResult>(nameof(ClearCache)).Post("", request);
}