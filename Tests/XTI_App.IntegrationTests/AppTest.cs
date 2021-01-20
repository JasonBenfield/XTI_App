using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.TestFakes;
using XTI_Configuration.Extensions;
using XTI_Core;

namespace XTI_App.IntegrationTests
{
    public sealed class AppTest
    {
        [Test]
        public async Task ShouldAddApp()
        {
            var input = await setup();
            Assert.That(input.App.Key(), Is.EqualTo(FakeInfo.AppKey));
        }

        [Test]
        public async Task ShouldGetAllApps()
        {
            var input = await setup();
            var apps = await input.Factory.Apps().All();
            Assert.That(apps.Select(a => a.Key()), Is.EquivalentTo(new[] { AppKey.Unknown, FakeInfo.AppKey }));
        }

        [Test]
        public async Task ShouldGetAppByKey()
        {
            var input = await setup();
            var app = await input.Factory.Apps().App(FakeInfo.AppKey);
            Assert.That(app.Key(), Is.EqualTo(FakeInfo.AppKey));
        }

        [Test]
        public async Task ShouldGetAppByID()
        {
            var input = await setup();
            var appByKey = await input.Factory.Apps().App(FakeInfo.AppKey);
            var appByID = await input.Factory.Apps().App(appByKey.ID.Value);
            Assert.That(appByID.Key(), Is.EqualTo(FakeInfo.AppKey));
        }

        private async Task<TestInput> setup()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration
                (
                    (hostContext, builder) => builder.UseXtiConfiguration(hostContext.HostingEnvironment, new string[] { })
                )
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddXtiTestServices(hostContext.Configuration);
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            await scope.ServiceProvider.Reset();
            var app = await scope.ServiceProvider.FakeApp();
            var input = new TestInput(scope.ServiceProvider, app);
            return input;
        }

        private sealed class TestInput
        {
            public TestInput(IServiceProvider sp, App app)
            {
                Factory = sp.GetService<AppFactory>();
                App = app;
                Clock = sp.GetService<Clock>();
            }

            public AppFactory Factory { get; }
            public App App { get; }
            public Clock Clock { get; }
        }
    }
}
