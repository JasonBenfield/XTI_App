using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.TestFakes;
using XTI_Core;
using XTI_Secrets.Fakes;
using XTI_Version;

namespace XTI_App.Tests
{
    public sealed class ManageVersionTester
    {
        private IServiceProvider sp;

        public ManageVersionOptions Options { get; private set; }

        public async Task Setup()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    services =>
                    {
                        services.AddServicesForTests();
                        services.AddSingleton<ManageVersionCommand>();
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
            var appKey = setup.App.Key();
            Options = new ManageVersionOptions
            {
                Command = "New",
                BranchName = "",
                AppName = appKey.Name.Value,
                AppType = appKey.Type.DisplayText,
                VersionType = AppVersionType.Values.Patch.DisplayText,
            };
        }

        public Task<AppVersion> Execute()
        {
            var command = Command();
            return command.Execute(Options);
        }

        public ManageVersionCommand Command() => sp.GetService<ManageVersionCommand>();
    }
}
