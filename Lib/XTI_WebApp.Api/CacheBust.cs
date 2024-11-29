using Microsoft.Extensions.Caching.Memory;
using XTI_App.Api;
using XTI_Core;

namespace XTI_WebApp.Api;

public sealed class CacheBust
{
    private readonly IMemoryCache cache;
    private readonly DefaultWebAppOptions options;
    private readonly XtiEnvironment xtiEnv;
    private readonly IAppContext appContext;
    private readonly XtiBasePath xtiBasePath;

    public CacheBust(IMemoryCache cache, DefaultWebAppOptions options, XtiEnvironment xtiEnv, IAppContext appContext, XtiBasePath xtiBasePath)
    {
        this.cache = cache;
        this.options = options;
        this.xtiEnv = xtiEnv;
        this.appContext = appContext;
        this.xtiBasePath = xtiBasePath;
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
                if (string.IsNullOrWhiteSpace(options.WebApp.CacheBust))
                {
                    var xtiPath = xtiBasePath.Value;
                    if (xtiPath.IsCurrentVersion())
                    {
                        var app = await appContext.App();
                        cacheBust = app.Version.VersionKey.DisplayText;
                        cache.Set(cacheKey, cacheBust);
                    }
                }
                else
                {
                    cacheBust = options.WebApp.CacheBust;
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