using Microsoft.Extensions.Caching.Memory;
using XTI_Credentials;

namespace XTI_App.Secrets;

public sealed class CachedSystemUserCredentials : ISystemUserCredentials
{
    private readonly IMemoryCache cache;
    private readonly ISystemUserCredentials source;

    public CachedSystemUserCredentials(IMemoryCache cache, ISystemUserCredentials source)
    {
        this.cache = cache;
        this.source = source;
    }

    public async Task<CredentialValue> Value()
    {
        if (!cache.TryGetValue<CredentialValue>("system_user", out var creds))
        {
            creds = await source.Value();
            cache.Set
            (
                "system_user",
                creds,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                }
            );
        }
        return creds ?? new CredentialValue("", "");
    }
}