using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XTI_App;
using XTI_Core.Fakes;
using XTI_Version;
using XTI_App.Fakes;
using XTI_Secrets.Fakes;
using XTI_App.EF;
using XTI_Core;
using XTI_App.TestFakes;

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
            Options = new ManageVersionOptions
            {
                Command = "New",
                BranchName = "",
                AppKey = setup.App.Key().Value,
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
