using System;
using System.Threading.Tasks;
using XTI_Git;
using XTI_Git.GitLib;
using XTI_GitHub;
using XTI_GitHub.Web;
using XTI_Secrets;
using XTI_Version;

namespace XTI_VersionTool
{
    public sealed class DefaultGitFactory : GitFactory
    {
        private readonly ISecretCredentialsFactory credentialsFactory;

        public DefaultGitFactory(ISecretCredentialsFactory credentialsFactory)
        {
            this.credentialsFactory = credentialsFactory;
        }

        public async Task<XtiGitHubRepository> CreateGitHubRepo(string ownerName, string repoName)
        {
            var gitHubRepo = new WebXtiGitHubRepository(ownerName, repoName);
            var credentials = credentialsFactory.Create("GitHub");
            var credentialsValue = await credentials.Value();
            gitHubRepo.UseCredentials(credentialsValue.UserName, credentialsValue.Password);
            return gitHubRepo;
        }

        public async Task<XtiGitRepository> CreateGitRepo()
        {
            var gitRepo = new GitLibXtiGitRepository(Environment.CurrentDirectory);
            var credentials = credentialsFactory.Create("GitHub");
            var credentialsValue = await credentials.Value();
            gitRepo.UseCredentials(credentialsValue.UserName, credentialsValue.Password);
            return gitRepo;
        }
    }
}
