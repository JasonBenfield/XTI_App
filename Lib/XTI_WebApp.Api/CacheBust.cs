using Microsoft.Extensions.Options;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;

namespace XTI_WebApp.Api;

public sealed class CacheBust
{
    private readonly WebAppOptions options;
    private readonly XtiEnvironment xtiEnv;
    private readonly IAppContext appContext;
    private readonly IXtiPathAccessor xtiPathAccessor;

    private string? value;

    public CacheBust(WebAppOptions options, XtiEnvironment xtiEnv, IAppContext appContext, IXtiPathAccessor xtiPathAccessor)
    {
        this.options = options;
        this.xtiEnv = xtiEnv;
        this.appContext = appContext;
        this.xtiPathAccessor = xtiPathAccessor;
    }

    public async Task<string> Value()
    {
        if (value == null)
        {
            if (string.IsNullOrWhiteSpace(options.CacheBust))
            {
                var xtiPath = xtiPathAccessor.Value();
                if (xtiEnv.IsDevelopmentOrTest())
                {
                    value = Guid.NewGuid().ToString("N");
                }
                else if (xtiPath.IsCurrentVersion())
                {
                    var appContext = await this.appContext.App();
                    value = appContext.Version.VersionKey.DisplayText;
                }
            }
            else
            {
                value = options.CacheBust;
            }
        }
        return value ?? "";
    }

    public async Task<string> Query()
    {
        var value = await Value();
        return string.IsNullOrWhiteSpace(value) ? "" : $"cacheBust={value}";
    }

    public override string ToString() => $"{nameof(CacheBust)} {value}";
}