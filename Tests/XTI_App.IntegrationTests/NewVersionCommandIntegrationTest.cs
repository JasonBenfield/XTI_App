using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_App.TestFakes;
using XTI_Configuration.Extensions;
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
                    services.AddXtiTestServices(hostContext.Configuration);
                    services.AddScoped<ManageVersionCommand>();
                })
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            if (envName == "Test")
            {
                await sp.Reset();
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
                    AppName = FakeInfo.AppKey.Name.Value,
                    AppType = FakeInfo.AppKey.Type.DisplayText,
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
