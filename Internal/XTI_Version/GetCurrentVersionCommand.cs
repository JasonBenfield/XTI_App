using System;
using System.Threading.Tasks;
using XTI_App;
using XTI_App.Abstractions;
using XTI_Git.Abstractions;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public sealed class GetCurrentVersionCommand : VersionCommand
    {
        private readonly AppFactory appFactory;
        private readonly GitFactory gitFactory;

        public GetCurrentVersionCommand(AppFactory appFactory, GitFactory gitFactory)
        {
            this.appFactory = appFactory;
            this.gitFactory = gitFactory;
        }

        public async Task Execute(VersionToolOptions options)
        {
            AppVersion currentVersion;
            if (!string.IsNullOrWhiteSpace(options.AppName) && !string.IsNullOrWhiteSpace(options.AppType))
            {
                var app = await getApp(options);
                currentVersion = await app.CurrentVersion();
            }
            else
            {
                var gitRepo = await gitFactory.CreateGitRepo();
                var currentBranchName = gitRepo.CurrentBranchName();
                var xtiBranchName = XtiBranchName.Parse(currentBranchName);
                if (xtiBranchName is XtiIssueBranchName issueBranchName && !string.IsNullOrWhiteSpace(options.RepoOwner))
                {
                    var gitHubRepo = await gitFactory.CreateGitHubRepo(options.RepoOwner, options.RepoName);
                    var issue = await gitHubRepo.Issue(issueBranchName.IssueNumber);
                    var milestoneName = XtiMilestoneName.Parse(issue.Milestone.Title);
                    var versionKey = AppVersionKey.Parse(milestoneName.Version.Key);
                    var version = await appFactory.Versions().Version(versionKey);
                    currentVersion = await version.Current();
                }
                else if (xtiBranchName is XtiVersionBranchName versionBranchName)
                {
                    var versionKey = AppVersionKey.Parse(versionBranchName.Version.Key);
                    var version = await appFactory.Versions().Version(versionKey);
                    currentVersion = await version.Current();
                }
                else
                {
                    throw new ArgumentException($"Branch '{currentBranchName}' is not valid");
                }
            }
            var output = new VersionOutput();
            await output.Output(currentVersion, options.OutputPath);
        }

        private Task<App> getApp(VersionToolOptions options)
        {
            var appKey = options.AppKey();
            return appFactory.Apps().App(appKey);
        }
    }
}
