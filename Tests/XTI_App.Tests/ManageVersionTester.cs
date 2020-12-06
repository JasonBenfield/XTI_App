using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XTI_App.TestFakes;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_Secrets.Fakes;
using XTI_Version;

namespace XTI_App.Tests
{
    public sealed class ManageVersionTester
    {
        private IServiceProvider sp;

        public AppFactory Factory { get; private set; }
        public App App { get; private set; }
        public FakeClock Clock { get; private set; }
        public ManageVersionOptions Options { get; private set; }

        public async Task Setup()
        {
            var services = new ServiceCollection();
            services.AddServicesForTests();
            services.AddSingleton<ManageVersionCommand>();
            services.AddFakeSecretCredentials();
            sp = services.BuildServiceProvider();
            var factory = sp.GetService<AppFactory>();
            var clock = sp.GetService<Clock>();
            var setup = new FakeAppSetup(factory, clock);
            await setup.Run();
            Factory = sp.GetService<AppFactory>();
            App = setup.App;
            Clock = sp.GetService<FakeClock>();
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
