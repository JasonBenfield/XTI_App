using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Api;
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
            Assert.That(app.Key(), Is.EqualTo(input.Options.AppKey), "Should add app");
            Assert.That(app.Title, Is.EqualTo(input.Options.Title), "Should set app title");
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

        [Test]
        public async Task ShouldAddUnknownApp()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(AppKey.Unknown, AppType.Values.NotFound);
            Assert.That(app.ID.IsValid(), Is.True, "Should add unknown app");
        }

        [Test]
        public async Task ShouldAddUnknownResourceGroup()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(AppKey.Unknown, AppType.Values.NotFound);
            var group = await app.Group(ResourceGroupName.Unknown);
            Assert.That(group.ID.IsValid(), Is.True, "Should add unknown resource group");
        }

        [Test]
        public async Task ShouldAddUnknownResource()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(AppKey.Unknown, AppType.Values.NotFound);
            var group = await app.Group(ResourceGroupName.Unknown);
            var resource = await group.Resource(ResourceName.Unknown);
            Assert.That(resource.ID.IsValid(), Is.True, "Should add unknown resource");
        }

        [Test]
        public async Task ShouldAddResourceGroupsFromAppTemplateGroups()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey, input.Options.AppType);
            var groups = (await app.Groups()).ToArray();
            Assert.That
            (
                groups.Select(g => g.Name()),
                Is.EquivalentTo(new[] { new ResourceGroupName("employee"), new ResourceGroupName("product") }),
                "Should add resource groups from template groups"
            );
        }

        [Test]
        public async Task ShouldAddResourcesFromAppTemplateActions()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey, input.Options.AppType);
            var group = await app.Group(new ResourceGroupName("employee"));
            var resources = (await group.Resources()).ToArray();
            Assert.That
            (
                resources.Select(r => r.Name()),
                Is.EquivalentTo(new[] { new ResourceName("AddEmployee"), new ResourceName("Employee") }),
                "Should add resources from template actions"
            );
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
            services.AddSingleton<IAppApiTemplateFactory, FakeAppApiTemplateFactory>();
            services.AddScoped<IAppSetup>(sp =>
            {
                var factory = sp.GetService<AppFactory>();
                var clock = sp.GetService<Clock>();
                var options = sp.GetService<FakeAppOptions>();
                var templateFactory = sp.GetService<IAppApiTemplateFactory>();
                var template = templateFactory.Create();
                return new DefaultAppSetup
                (
                    factory,
                    clock,
                    template,
                    options.Title,
                    options.RoleNames.Values()
                );
            });
            return new TestInput(services.BuildServiceProvider());
        }

        private sealed class FakeAppOptions
        {
            public AppKey AppKey { get; } = FakeAppKey.AppKey;
            public AppType AppType { get; } = AppType.Values.Service;
            public string Title { get; set; } = "Fake Title";
            public AppRoleNames RoleNames { get; } = FakeAppRoles.Instance;
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
