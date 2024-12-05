using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

internal sealed class AppVersionKeyAccessor
{
    public AppVersionKeyAccessor(DefaultAppOptions options, IHostEnvironment hostEnv)
    {
        Value = GetVersionKey(options, hostEnv);
    }

    private static AppVersionKey GetVersionKey(DefaultAppOptions options, IHostEnvironment hostEnv)
    {
        var versionKey = AppVersionKey.Parse(options.VersionKey);
        if (versionKey.IsNone())
        {
            var appDir = new DirectoryInfo(hostEnv.ContentRootPath);
            versionKey = AppVersionKey.Parse(appDir.Name);
            if (versionKey.Equals(AppVersionKey.None))
            {
                versionKey = AppVersionKey.Current;
            }
        }
        else
        {
            versionKey = AppVersionKey.Current;
        }
        return versionKey;
    }

    public AppVersionKey Value { get; }
}
