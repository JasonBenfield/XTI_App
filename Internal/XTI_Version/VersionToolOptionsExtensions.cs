using XTI_App.Abstractions;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public static class VersionToolOptionsExtensions
    {
        public static AppKey AppKey(this VersionToolOptions options)
        {
            var appType = string.IsNullOrWhiteSpace(options.AppType)
                ? AppType.Values.WebApp
                : AppType.Values.Value(options.AppType);
            var appKey = new AppKey(options.AppName, appType);
            return appKey;
        }

    }
}
