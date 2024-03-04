using Microsoft.Extensions.Caching.Memory;
using XTI_App.Api;
using XTI_Core;

namespace XTI_WebApp.Api;

public sealed class CacheBust
{
    private readonly IMemoryCache cache;
    private readonly WebAppOptions options;
    private readonly XtiEnvironment xtiEnv;
    private readonly IAppContext appContext;
    private readonly IXtiPathAccessor xtiPathAccessor;

    public CacheBust(IMemoryCache cache, WebAppOptions options, XtiEnvironment xtiEnv, IAppContext appContext, IXtiPathAccessor xtiPathAccessor)
    {
        this.cache = cache;
        this.options = options;
        this.xtiEnv = xtiEnv;
        this.appContext = appContext;
        this.xtiPathAccessor = xtiPathAccessor;
    }

    public async Task<string> Value()
    {
        string? cacheBust;
        if (xtiEnv.IsDevelopmentOrTest())
        {
            cacheBust = Guid.NewGuid().ToString("N");
        }
        else
        {
            const string cacheKey = "XTI_CacheBust";
            if (!cache.TryGetValue(cacheKey, out cacheBust))
            {
                if (string.IsNullOrWhiteSpace(options.CacheBust))
                {
                    var xtiPath = xtiPathAccessor.Value();
                    if (xtiPath.IsCurrentVersion())
                    {
                        var app = await appContext.App();
                        cacheBust = app.Version.VersionKey.DisplayText;
                        cache.Set(cacheKey, cacheBust);
                    }
                }
                else
                {
                    cacheBust = options.CacheBust;
                }
            }
        }
        return cacheBust ?? "";
    }

    public async Task<string> Query()
    {
        var value = await Value();
        return string.IsNullOrWhiteSpace(value) ? "" : $"cacheBust={value}";
    }
}