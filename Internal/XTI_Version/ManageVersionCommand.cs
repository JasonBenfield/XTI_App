using System;
using System.Threading.Tasks;
using XTI_App;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public sealed class ManageVersionCommand
    {
        private readonly AppFactory factory;
        private readonly Clock clock;

        public ManageVersionCommand(AppFactory factory, Clock clock)
        {
            this.factory = factory;
            this.clock = clock;
        }

        public async Task<AppVersion> Execute(VersionToolOptions options)
        {
            AppVersion version;
            if (options.Command.Equals("New", StringComparison.OrdinalIgnoreCase))
            {
                version = await startNewVersion(options);
            }
            else
            {
                var versionKey = AppVersionKey.Parse(options.VersionKey);
                version = await factory.Versions().Version(versionKey);
                if (options.Command.Equals("BeginPublish", StringComparison.OrdinalIgnoreCase))
                {
                    if (!version.IsNew() && !version.IsPublishing())
                    {
                        throw new PublishVersionException($"Unable to begin publishing version '{options.VersionKey}' when it's status is not 'New' or 'Publishing'");
                    }
                    await version.Publishing();
                }
                else if (options.Command.Equals("EndPublish", StringComparison.OrdinalIgnoreCase))
                {
                    await version.Published();
                }
                else if (options.Command.Equals("GetCurrent", StringComparison.OrdinalIgnoreCase))
                {
                    version = await version.Current();
                }
                else if (!options.Command.Equals("GetVersion", StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotSupportedException($"Command '{options.Command}' is not supported");
                }
            }
            return version;
        }

        private async Task<AppVersion> startNewVersion(VersionToolOptions options)
        {
            var app = await getApp(options);
            AppVersion version;
            var versionType = AppVersionType.Values.Value(options.VersionType);
            if (versionType.Equals(AppVersionType.Values.Major))
            {
                version = await app.StartNewMajorVersion(clock.Now());
            }
            else if (versionType.Equals(AppVersionType.Values.Minor))
            {
                version = await app.StartNewMinorVersion(clock.Now());
            }
            else if (versionType.Equals(AppVersionType.Values.Patch))
            {
                version = await app.StartNewPatch(clock.Now());
            }
            else
            {
                version = null;
            }
            return version;
        }

        private Task<App> getApp(VersionToolOptions options)
        {
            var appKey = getAppKey(options);
            return factory.Apps().App(appKey);
        }

        private static AppKey getAppKey(VersionToolOptions options)
        {
            var appType = string.IsNullOrWhiteSpace(options.AppType)
                ? AppType.Values.WebApp
                : AppType.Values.Value(options.AppType);
            var appKey = new AppKey(options.AppName, appType);
            return appKey;
        }
    }
}
