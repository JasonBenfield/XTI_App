using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.TestFakes;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.Tests
{
    public sealed class DefaultAppSetupTest
    {
        [Test]
        public async Task ShouldAddWebApp()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey, input.Options.AppType);
            Assert.That(app.Exists(), Is.True, "Should add app");
            Assert.That(app.Title, Is.EqualTo(input.Options.Title), "Should set app title");
        }

        [Test]
        public async Task ShouldAddPackage()
        {
            var input = setup();
            input.Options.AppType = AppType.Values.Package;
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey, input.Options.AppType);
            Assert.That(app.Exists(), Is.True, "Should add package");
        }

        [Test]
        public async Task ShouldAddCurrentVersion()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey, input.Options.AppType);
            var currentVersion = await app.CurrentVersion();
            Assert.That(currentVersion.IsCurrent(), Is.True, "Should add current version");
        }

        [Test]
        public async Task ShouldAddRoles()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey, input.Options.AppType);
            var roleNames = FakeAppRoles.Instance.Values();
            var appRoles = await app.Roles();
            Assert.That(appRoles.Select(r => r.Name()), Is.EquivalentTo(roleNames), "Should add role names from app role names");
        }

        [Test]
        public async Task ShouldChangeAppTitle()
        {
            var input = setup();
            await execute(input);
            input.Options.Title = "New Title";
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey, input.Options.AppType);
            Assert.That(app.Title, Is.EqualTo(input.Options.Title), "Should set app title");
        }

        private async Task execute(TestInput input)
        {
            using var scope = input.Services.CreateScope();
            var setup = scope.ServiceProvider.GetService<IAppSetup>();
            await setup.Run();
        }

        private TestInput setup()
        {
            var services = new ServiceCollection();
            services.AddServicesForTests();
            services.AddSingleton<FakeAppOptions>();
            services.AddScoped<IAppSetup>(sp =>
            {
                var factory = sp.GetService<AppFactory>();
                var clock = sp.GetService<Clock>();
                var options = sp.GetService<FakeAppOptions>();
                return new DefaultAppSetup
                (
                    factory,
                    clock,
                    options.AppKey,
                    options.AppType,
                    options.Title,
                    FakeAppRoles.Instance.Values()
                );
            });
            return new TestInput(services.BuildServiceProvider());
        }

        private sealed class FakeAppOptions
        {
            public AppKey AppKey { get; set; } = new AppKey("Fake");
            public AppType AppType { get; set; } = AppType.Values.WebApp;
            public string Title { get; set; } = "Fake Title";
            public AppRoleNames RoleNames { get; set; } = FakeAppRoles.Instance;
        }

        private sealed class TestInput
        {
            public TestInput(ServiceProvider sp)
            {
                Services = sp;
                Factory = sp.GetService<AppFactory>();
                Clock = sp.GetService<FakeClock>();
                Options = sp.GetService<FakeAppOptions>();
            }

            public AppFactory Factory { get; }
            public FakeClock Clock { get; }
            public FakeAppOptions Options { get; }
            public IServiceProvider Services { get; }
        }
    }
}
