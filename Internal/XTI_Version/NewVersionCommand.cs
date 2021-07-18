using System;
using System.Threading.Tasks;
using XTI_App;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_Git.Abstractions;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public sealed class NewVersionCommand : VersionCommand
    {
        private readonly AppFactory appFactory;
        private readonly Clock clock;
        private readonly GitFactory gitFactory;

        public NewVersionCommand(AppFactory appFactory, Clock clock, GitFactory gitFactory)
        {
            this.appFactory = appFactory;
            this.clock = clock;
            this.gitFactory = gitFactory;
        }

        public async Task Execute(VersionToolOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.AppName)) { throw new ArgumentException("App Name is required"); }
            if (string.IsNullOrWhiteSpace(options.AppType)) { throw new ArgumentException("App Type is required"); }
            if (string.IsNullOrWhiteSpace(options.RepoOwner)) { throw new ArgumentException("Repo Owner is required"); }
            if (string.IsNullOrWhiteSpace(options.RepoName)) { throw new ArgumentException("Repo Name is required"); }
            var gitRepo = await gitFactory.CreateGitRepo();
            var currentBranchName = gitRepo.CurrentBranchName();
            var gitHubRepo = await gitFactory.CreateGitHubRepo(options.RepoOwner, options.RepoName);
            var defaultBranchName = await gitHubRepo.DefaultBranchName();
            if (!currentBranchName.Equals(defaultBranchName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Current branch '{currentBranchName}' is not the default branch '{defaultBranchName}'");
            }
            await appFactory.Transaction(async () =>
            {
                var version = await startNewVersion(options);
                var gitVersion = new XtiGitVersion(version.Type().DisplayText, version.Key().DisplayText);
                await gitHubRepo.CreateNewVersion(gitVersion);
                var newVersionBranchName = gitVersion.BranchName();
                gitRepo.CheckoutBranch(newVersionBranchName.Value);
            });
        }

        private async Task<AppVersion> startNewVersion(VersionToolOptions options)
        {
            var app = await getApp(options);
            if (!app.Key().Name.Equals(options.AppName))
            {
                var appKey = options.AppKey();
                app = await appFactory.Apps().Add(appKey, appKey.Name.DisplayText, clock.Now());
            }
            var currentVersion = await app.CurrentVersion();
            if (!currentVersion.IsCurrent())
            {
                currentVersion = await app.StartNewMajorVersion(clock.Now());
                await currentVersion.Publishing();
                await currentVersion.Published();
            }
            AppVersion version;
            var versionType = AppVersionType.Values.Value(options.VersionType);
            if (versionType == null)
            {
                throw new PublishVersionException($"Version type '{options.VersionType}' is not valid");
            }
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
            var appKey = options.AppKey();
            return appFactory.Apps().App(appKey);
        }

    }
}
