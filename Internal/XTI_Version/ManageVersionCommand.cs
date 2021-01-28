﻿using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using XTI_App;
using XTI_Core;

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

        public async Task<AppVersion> Execute(ManageVersionOptions options)
        {
            AppVersion version;
            if (options.Command.Equals("New", StringComparison.OrdinalIgnoreCase))
            {
                version = await startNewVersion(options);
            }
            else
            {
                var xtiVersionBranch = new XtiVersionBranch(options.BranchName);
                var versionKeyText = xtiVersionBranch.VersionKey();
                var versionKey = AppVersionKey.Parse(versionKeyText);
                version = await factory.Versions().Version(versionKey);
                if (options.Command.Equals("BeginPublish", StringComparison.OrdinalIgnoreCase))
                {
                    if (!version.Key().Equals(versionKey))
                    {
                        var appKey = getAppKey(options);
                        var app = await getApp(options);
                        if (!app.Key().Equals(appKey))
                        {
                            app = await factory.Apps().Add(appKey, appKey.Name.DisplayText, clock.Now());
                        }
                        var versionType = xtiVersionBranch.VersionType();
                        version = await app.NewVersion(versionKey, versionType, new Version(1, 0, 0), clock.Now());
                    }
                    if (!version.IsNew() && !version.IsPublishing())
                    {
                        throw new PublishVersionException($"Unable to begin publishing version {versionKeyText} when it's status is not 'New' or 'Publishing'");
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
            if (!string.IsNullOrWhiteSpace(options.OutputPath))
            {
                var dirPath = Path.GetDirectoryName(options.OutputPath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                using var writer = new StreamWriter(options.OutputPath, false);
                writer.WriteLine
                (
                    JsonSerializer.Serialize
                    (
                        new VersionRecord
                        {
                            Key = version.Key().Value,
                            Type = version.Type().DisplayText,
                            Version = version.Version().ToString()
                        }
                    )
                );
            }
            return version;
        }

        private async Task<AppVersion> startNewVersion(ManageVersionOptions options)
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

        private Task<App> getApp(ManageVersionOptions options)
        {
            var appKey = getAppKey(options);
            return factory.Apps().App(appKey);
        }

        private static AppKey getAppKey(ManageVersionOptions options)
        {
            var appType = string.IsNullOrWhiteSpace(options.AppType)
                ? AppType.Values.WebApp
                : AppType.Values.Value(options.AppType);
            var appKey = new AppKey(options.AppName, appType);
            return appKey;
        }

        private class VersionRecord
        {
            public string Key { get; set; }
            public string Type { get; set; }
            public string Version { get; set; }
        }

    }
}
