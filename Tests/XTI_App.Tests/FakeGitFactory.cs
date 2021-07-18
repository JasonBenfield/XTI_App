using System.Threading.Tasks;
using XTI_Git;
using XTI_Git.Fakes;
using XTI_GitHub;
using XTI_GitHub.Fakes;
using XTI_Version;

namespace XTI_App.Tests
{
    public sealed class FakeGitFactory : GitFactory
    {
        private FakeXtiGitHubRepository gitHubRepo;

        public Task<XtiGitHubRepository> CreateGitHubRepo(string ownerName, string repoName)
        {
            XtiGitHubRepository repo = gitHubRepo ?? (gitHubRepo = new FakeXtiGitHubRepository(ownerName));
            return Task.FromResult(repo);
        }

        private FakeXtiGitRepository gitRepo;

        public Task<XtiGitRepository> CreateGitRepo()
        {
            XtiGitRepository repo = gitRepo ?? (gitRepo = new FakeXtiGitRepository());
            return Task.FromResult(repo);
        }
    }
}
