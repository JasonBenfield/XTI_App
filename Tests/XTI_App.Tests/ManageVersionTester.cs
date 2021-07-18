using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.TestFakes;
using XTI_Core;
using XTI_Git.Abstractions;
using XTI_Secrets.Fakes;
using XTI_Version;
using XTI_VersionToolApi;

namespace XTI_App.Tests
{
    public sealed class ManageVersionTester
    {
        private IServiceProvider sp;

        public VersionToolOptions Options { get; private set; }
        public App App { get; private set; }

        public async Task Setup()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    services =>
                    {
                        services.AddServicesForTests();
                        services.AddScoped<GitFactory, FakeGitFactory>();
                        services.AddScoped<VersionCommandFactory>();
                        services.AddFakeSecretCredentials();
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            sp = scope.ServiceProvider;
            var factory = sp.GetService<AppFactory>();
            var clock = sp.GetService<Clock>();
            var setup = new FakeAppSetup(factory, clock);
            await setup.Run(AppVersionKey.Current);
            App = setup.App;
            var appKey = setup.App.Key();
            Options = new VersionToolOptions();
            Options.CommandNewVersion
            (
                appKey.Name.Value,
                appKey.Type.DisplayText,
                AppVersionType.Values.Patch.DisplayText,
                "JasonBenfield",
                "XTI_App"
            );
            var gitFactory = sp.GetService<GitFactory>();
            var gitHubRepo = await gitFactory.CreateGitHubRepo("JasonBenfield", "XTI_App");
            var defaultBranchName = await gitHubRepo.DefaultBranchName();
            var gitRepo = await gitFactory.CreateGitRepo();
            gitRepo.CheckoutBranch(defaultBranchName);
        }

        public Task Execute()
        {
            var command = Command();
            return command.Execute(Options);
        }

        public VersionCommand Command()
        {
            var commandFactory = sp.GetService<VersionCommandFactory>();
            var commandName = VersionCommandName.FromValue(Options.Command);
            var command = commandFactory.Create(commandName);
            return command;
        }

        public async Task Checkout(AppVersion version)
        {
            var gitFactory = sp.GetService<GitFactory>();
            var gitRepo = await gitFactory.CreateGitRepo();
            var versionBranchName = new XtiVersionBranchName(new XtiGitVersion(version.Type().DisplayText, version.Key().DisplayText));
            gitRepo.CheckoutBranch(versionBranchName.Value);
        }
    }
}
