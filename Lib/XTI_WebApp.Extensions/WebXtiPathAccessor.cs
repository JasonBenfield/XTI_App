using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;

namespace XTI_WebApp.Extensions;

public sealed class WebXtiPathAccessor : IXtiPathAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly AppKey appKey;
    private readonly IWebHostEnvironment hostEnv;

    public WebXtiPathAccessor(IHttpContextAccessor httpContextAccessor, AppKey appKey, IWebHostEnvironment hostEnv)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.appKey = appKey;
        this.hostEnv = hostEnv;
    }

    public XtiPath Value()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        var path = $"{request?.PathBase}{request?.Path}";
        if (string.IsNullOrWhiteSpace(path))
        {
            var dirName = Path.GetDirectoryName(hostEnv.ContentRootPath) ?? "";
            var versionKey = AppVersionKey.Parse(dirName);
            if (versionKey.Equals(AppVersionKey.None))
            {
                versionKey = AppVersionKey.Current;
            }
            path = $"{appKey.Name.DisplayText}/{versionKey.DisplayText}";
        }
        return XtiPath.Parse(path);
    }
}