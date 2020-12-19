using MainDB.EF;
using MainDB.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.TestFakes;
using XTI_Configuration.Extensions;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.IntegrationTests
{
    public sealed class AppTest
    {
        [Test]
        public async Task ShouldAddApp()
        {
            var input = await setup();
            Assert.That(input.App.Key(), Is.EqualTo(FakeAppKey.AppKey));
        }

        [Test]
        public async Task ShouldGetAllApps()
        {
            var input = await setup();
            var apps = await input.Factory.Apps().All();
            Assert.That(apps.Select(a => a.Key()), Is.EquivalentTo(new[] { AppKey.Unknown, FakeAppKey.AppKey }));
        }

        [Test]
        public async Task ShouldGetAppByKey()
        {
            var input = await setup();
            var app = await input.Factory.Apps().App(FakeAppKey.AppKey);
            Assert.That(app.Key(), Is.EqualTo(FakeAppKey.AppKey));
        }

        [Test]
        public async Task ShouldGetAppByID()
        {
            var input = await setup();
            var appByKey = await input.Factory.Apps().App(FakeAppKey.AppKey);
            var appByID = await input.Factory.Apps().App(appByKey.ID.Value);
            Assert.That(appByID.Key(), Is.EqualTo(FakeAppKey.AppKey));
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
                        services.AddAppDbContextForSqlServer(hostContext.Configuration);
                        services.AddScoped<Clock, FakeClock>();
                        services.AddScoped<AppFactory>();
                        services.AddScoped<MainDbReset>();
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var factory = scope.ServiceProvider.GetService<AppFactory>();
            var appDbReset = scope.ServiceProvider.GetService<MainDbReset>();
            await appDbReset.Run();
            var clock = scope.ServiceProvider.GetService<Clock>();
            var setup = new FakeAppSetup(factory, clock);
            await setup.Run();
            var input = new TestInput(scope.ServiceProvider, setup.App);
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
