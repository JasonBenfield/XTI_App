using MainDB.EF;
using MainDB.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_App.TestFakes;
using XTI_Configuration.Extensions;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_Version;

namespace XTI_App.IntegrationTests
{
    public sealed class NewVersionCommandIntegrationTest
    {
        [Test]
        public async Task ShouldCreateNewPatch()
        {
            var input = await setup();
            input.Options.VersionType = AppVersionType.Values.Patch.DisplayText;
            var newVersion = await execute(input);
            Assert.That(newVersion?.IsPatch(), Is.True, "Should start new patch");
        }

        [Test]
        public async Task ShouldCreateNewMinorVersion()
        {
            var input = await setup();
            input.Options.VersionType = AppVersionType.Values.Minor.DisplayText;
            var newVersion = await execute(input);
            Assert.That(newVersion?.IsMinor(), Is.True, "Should start new minor version");
        }

        [Test]
        public async Task ShouldCreateNewMajorVersion()
        {
            var input = await setup();
            input.Options.VersionType = AppVersionType.Values.Major.DisplayText;
            var newVersion = await execute(input);
            Assert.That(newVersion?.IsMajor(), Is.True, "Should start new major version");
        }

        [Test]
        public async Task Configure()
        {
            var input = await setup("Production");
            input.Options.AppName = "TempLog";
            input.Options.AppType = AppType.Values.Service.DisplayText;
            input.Options.VersionType = AppVersionType.Values.Minor.DisplayText;
            var appSetup = new SingleAppSetup
            (
                input.Services.GetService<AppFactory>(),
                input.Services.GetService<Clock>(),
                new AppKey(input.Options.AppName, AppType.Values.Value(input.Options.AppType)),
                "",
                new AppRoleName[] { }
            );
            await appSetup.Run();
            var newVersion = await execute(input);
            //Assert.That(newVersion?.IsMajor(), Is.True, "Should start new major version");
        }

        private async Task<TestInput> setup(string envName = "Test")
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", envName);
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.UseXtiConfiguration(hostingContext.HostingEnvironment, new string[] { });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddAppDbContextForSqlServer(hostContext.Configuration);
                    services.AddScoped<Clock, FakeClock>();
                    services.AddScoped<AppFactory>();
                    services.AddScoped<ManageVersionCommand>();
                    services.AddScoped<MainDbReset>();
                })
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var factory = sp.GetService<AppFactory>();
            var clock = sp.GetService<Clock>();
            if (envName == "Test")
            {
                var appDbReset = sp.GetService<MainDbReset>();
                await appDbReset.Run();
                var setup = new FakeAppSetup(factory, clock);
                await setup.Run();
            }
            var input = new TestInput(sp);
            return input;
        }

        private Task<AppVersion> execute(TestInput input)
        {
            var command = input.Command();
            return command.Execute(input.Options);
        }

        private sealed class TestInput
        {
            public TestInput(IServiceProvider sp)
            {
                Services = sp;
                Factory = sp.GetService<AppFactory>();
                Options = new ManageVersionOptions
                {
                    Command = "New",
                    AppName = FakeAppKey.AppName.Value,
                    AppType = FakeAppKey.AppKey.Type.DisplayText,
                    VersionType = AppVersionType.Values.Patch.DisplayText
                };
            }

            public AppFactory Factory { get; }
            public ManageVersionOptions Options { get; }
            public IServiceProvider Services { get; }

            public ManageVersionCommand Command() => Services.GetService<ManageVersionCommand>();
        }
    }
}
