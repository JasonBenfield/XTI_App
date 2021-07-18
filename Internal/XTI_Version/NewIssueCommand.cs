using System;
using System.Threading.Tasks;
using XTI_Git.Abstractions;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public sealed class NewIssueCommand : VersionCommand
    {
        private readonly GitFactory gitFactory;

        public NewIssueCommand(GitFactory gitFactory)
        {
            this.gitFactory = gitFactory;
        }

        public async Task Execute(VersionToolOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.RepoOwner)) { throw new ArgumentException("Repo Owner is required"); }
            if (string.IsNullOrWhiteSpace(options.RepoName)) { throw new ArgumentException("Repo Name is required"); }
            if (string.IsNullOrWhiteSpace(options.IssueTitle)) { throw new ArgumentException("Issue Title is required"); }
            var gitRepo = await gitFactory.CreateGitRepo();
            var currentBranchName = gitRepo.CurrentBranchName();
            var gitHubRepo = await gitFactory.CreateGitHubRepo(options.RepoOwner, options.RepoName);
            XtiGitVersion xtiGitVersion;
            var xtiBranchName = XtiBranchName.Parse(currentBranchName);
            if (xtiBranchName is XtiIssueBranchName issueBranchName)
            {
                if (options.StartIssue)
                {
                    throw new ArgumentException("Unable to start issue when not a version branch");
                }
                var branchIssue = await gitHubRepo.Issue(issueBranchName.IssueNumber);
                var xtiMilestoneName = XtiMilestoneName.Parse(branchIssue.Milestone.Title);
                xtiGitVersion = xtiMilestoneName.Version;
            }
            else if (xtiBranchName is XtiVersionBranchName versionBranchName)
            {
                xtiGitVersion = versionBranchName.Version;
            }
            else
            {
                throw new ArgumentException($"Branch '{currentBranchName}' is not an issue branch or a version branch");
            }
            var issue = await gitHubRepo.CreateIssue(xtiGitVersion, options.IssueTitle);
            if (options.StartIssue)
            {
                gitRepo.CheckoutBranch(issue.BranchName().Value);
            }
        }
    }
}
