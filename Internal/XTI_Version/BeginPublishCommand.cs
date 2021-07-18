using System;
using System.Threading.Tasks;
using XTI_App;
using XTI_App.Abstractions;
using XTI_Git.Abstractions;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public sealed class BeginPublishCommand : VersionCommand
    {
        private readonly AppFactory appFactory;
        private readonly GitFactory gitFactory;

        public BeginPublishCommand(AppFactory appFactory, GitFactory gitFactory)
        {
            this.appFactory = appFactory;
            this.gitFactory = gitFactory;
        }

        public async Task Execute(VersionToolOptions options)
        {
            var gitRepo = await gitFactory.CreateGitRepo();
            var branchName = gitRepo.CurrentBranchName();
            var xtiBranchName = XtiBranchName.Parse(branchName);
            if (xtiBranchName is not XtiVersionBranchName versionBranchName)
            {
                throw new ArgumentException($"Branch '{branchName}' is not a version branch");
            }
            var versionKey = AppVersionKey.Parse(versionBranchName.Version.Key);
            var version = await appFactory.Versions().Version(versionKey);
            if (!version.IsNew() && !version.IsPublishing())
            {
                throw new PublishVersionException($"Unable to begin publishing version '{versionKey.DisplayText}' when it's status is not 'New' or 'Publishing'");
            }
            await version.Publishing();
            var output = new VersionOutput();
            await output.Output(version, options.OutputPath);
        }
    }
}
