using System;
using System.Threading.Tasks;
using XTI_App;
using XTI_App.Abstractions;
using XTI_Git.Abstractions;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public sealed class GetVersionCommand : VersionCommand
    {
        private readonly AppFactory appFactory;
        private readonly GitFactory gitFactory;

        public GetVersionCommand(AppFactory appFactory, GitFactory gitFactory)
        {
            this.appFactory = appFactory;
            this.gitFactory = gitFactory;
        }

        public async Task Execute(VersionToolOptions options)
        {
            AppVersion version;
            var gitRepo = await gitFactory.CreateGitRepo();
            var currentBranchName = gitRepo.CurrentBranchName();
            var xtiBranchName = XtiBranchName.Parse(currentBranchName);
            if (xtiBranchName is XtiIssueBranchName issueBranchName && !string.IsNullOrWhiteSpace(options.RepoOwner))
            {
                var gitHubRepo = await gitFactory.CreateGitHubRepo(options.RepoOwner, options.RepoName);
                var issue = await gitHubRepo.Issue(issueBranchName.IssueNumber);
                var milestoneName = XtiMilestoneName.Parse(issue.Milestone.Title);
                var versionKey = AppVersionKey.Parse(milestoneName.Version.Key);
                version = await appFactory.Versions().Version(versionKey);
            }
            else if (xtiBranchName is XtiVersionBranchName versionBranchName)
            {
                var versionKey = AppVersionKey.Parse(versionBranchName.Version.Key);
                version = await appFactory.Versions().Version(versionKey);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(options.AppName)) { throw new ArgumentException("App Name is required"); }
                if (string.IsNullOrWhiteSpace(options.AppType)) { throw new ArgumentException("App Type is required"); }
                var app = await getApp(options);
                version = await app.CurrentVersion();
            }
            var output = new VersionOutput();
            await output.Output(version, options.OutputPath);
        }

        private Task<App> getApp(VersionToolOptions options)
        {
            var appKey = options.AppKey();
            return appFactory.Apps().App(appKey);
        }
    }
}
