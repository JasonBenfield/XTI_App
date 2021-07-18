using System;
using System.Threading.Tasks;
using XTI_Git.Abstractions;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public sealed class StartIssueCommand : VersionCommand
    {
        private readonly GitFactory gitFactory;

        public StartIssueCommand(GitFactory gitFactory)
        {
            this.gitFactory = gitFactory;
        }

        public async Task Execute(VersionToolOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.RepoOwner)) { throw new ArgumentException("Repo Owner is required"); }
            if (string.IsNullOrWhiteSpace(options.RepoName)) { throw new ArgumentException("Repo Name is required"); }
            if (options.IssueNumber <= 0) { throw new ArgumentException("Issue Number is required"); }
            var gitRepo = await gitFactory.CreateGitRepo();
            var currentBranchName = gitRepo.CurrentBranchName();
            var xtiBranchName = XtiBranchName.Parse(currentBranchName);
            if (xtiBranchName is not XtiVersionBranchName)
            {
                throw new ArgumentException($"Branch '{currentBranchName}' is not a version branch");
            }
            var gitHubRepo = await gitFactory.CreateGitHubRepo(options.RepoOwner, options.RepoName);
            var issue = await gitHubRepo.Issue(options.IssueNumber);
            gitRepo.CheckoutBranch(issue.BranchName().Value);
        }
    }
}
