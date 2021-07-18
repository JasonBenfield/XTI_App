using System;
using System.Threading.Tasks;
using XTI_Git.Abstractions;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public sealed class IssuesCommand : VersionCommand
    {
        private readonly GitFactory gitFactory;

        public IssuesCommand(GitFactory gitFactory)
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
            var gitHubRepo = await gitFactory.CreateGitHubRepo(options.RepoOwner, options.RepoName);
            XtiMilestoneName milestoneName;
            var xtiBranchName = XtiBranchName.Parse(currentBranchName);
            if (xtiBranchName is XtiIssueBranchName issueBranchName)
            {
                var issue = await gitHubRepo.Issue(issueBranchName.IssueNumber);
                milestoneName = XtiMilestoneName.Parse(issue.Milestone.Title);
            }
            else if (xtiBranchName is XtiVersionBranchName versionBranchName)
            {
                milestoneName = versionBranchName.Version.MilestoneName();
            }
            else
            {
                throw new ArgumentException($"Branch '{currentBranchName}' is not an issue branch or a version branch");
            }
            var milestone = await gitHubRepo.Milestone(milestoneName.Value);
            var issues = await gitHubRepo.OpenIssues(milestone);
            foreach (var issue in issues)
            {
                Console.WriteLine($"{issue.Number}: {issue.Title}");
            }
        }
    }
}
