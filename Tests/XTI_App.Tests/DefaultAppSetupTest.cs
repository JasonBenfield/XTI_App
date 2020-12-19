using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            var app = await input.Factory.Apps().App(input.Options.AppKey);
            Assert.That(app.Key(), Is.EqualTo(input.Options.AppKey), "Should add app");
            Assert.That(app.Title, Is.EqualTo(input.Options.Title), "Should set app title");
        }

        [Test]
        public async Task ShouldAddCurrentVersion()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey);
            var currentVersion = await app.CurrentVersion();
            Assert.That(currentVersion.IsCurrent(), Is.True, "Should add current version");
        }

        [Test]
        public async Task ShouldAddRoles()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey);
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
            var app = await input.Factory.Apps().App(input.Options.AppKey);
            Assert.That(app.Title, Is.EqualTo(input.Options.Title), "Should set app title");
        }

        [Test]
        public async Task ShouldAddUnknownApp()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(AppKey.Unknown);
            Assert.That(app.ID.IsValid(), Is.True, "Should add unknown app");
        }

        [Test]
        public async Task ShouldAddUnknownResourceGroup()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(AppKey.Unknown);
            var group = await app.ResourceGroup(ResourceGroupName.Unknown);
            Assert.That(group.ID.IsValid(), Is.True, "Should add unknown resource group");
        }

        [Test]
        public async Task ShouldAddUnknownResource()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(AppKey.Unknown);
            var group = await app.ResourceGroup(ResourceGroupName.Unknown);
            var resource = await group.Resource(ResourceName.Unknown);
            Assert.That(resource.ID.IsValid(), Is.True, "Should add unknown resource");
        }

        [Test]
        public async Task ShouldAddResourceGroupsFromAppTemplateGroups()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey);
            var groups = (await app.ResourceGroups()).ToArray();
            Assert.That
            (
                groups.Select(g => g.Name()),
                Is.EquivalentTo(new[] { new ResourceGroupName("employee"), new ResourceGroupName("product"), new ResourceGroupName("login"), new ResourceGroupName("home") }),
                "Should add resource groups from template groups"
            );
        }

        [Test]
        public async Task ShouldAddResourcesFromAppTemplateActions()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey);
            var group = await app.ResourceGroup(new ResourceGroupName("employee"));
            var resources = (await group.Resources()).ToArray();
            Assert.That
            (
                resources.Select(r => r.Name()),
                Is.EquivalentTo(new[] { new ResourceName("AddEmployee"), new ResourceName("Employee") }),
                "Should add resources from template actions"
            );
        }

        [Test]
        public async Task ShouldAddDefaultModifierCategory()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey);
            var modCategory = await app.ModCategory(ModifierCategoryName.Default);
            Assert.That
            (
                modCategory.Name().Equals(ModifierCategoryName.Default),
                Is.True,
                "Should add default modifier category"
            );
        }

        [Test]
        public async Task ShouldAddDefaultModifierCategoryToApp()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey);
            var category = await app.ModCategory(ModifierCategoryName.Default);
            Assert.That(category.ID.IsValid(), Is.True, "Should add default modifier to app");
        }

        [Test]
        public async Task ShouldSetModifierCategoryForGroup()
        {
            var input = setup();
            await execute(input);
            var app = await input.Factory.Apps().App(input.Options.AppKey);
            var employeeGroup = await app.ResourceGroup(new ResourceGroupName("Employee"));
            var modifiers = (await employeeGroup.Modifiers()).ToArray();
            Assert.That
            (
                modifiers.Select(m => m.DisplayText),
                Is.EquivalentTo(new[] { "IT", "HR" }),
                "Should add modifiers from group"
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
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    services =>
                    {
                        services.AddServicesForTests();
                        services.AddSingleton<FakeAppOptions>();
                        services.AddSingleton<IAppApiTemplateFactory, FakeAppApiTemplateFactory>();
                        services.AddScoped<IAppSetup>(sp =>
                        {
                            var factory = sp.GetService<AppFactory>();
                            var clock = sp.GetService<Clock>();
                            var options = sp.GetService<FakeAppOptions>();
                            return new FakeAppSetup
                            (
                                factory,
                                clock,
                                options
                            );
                        });
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            return new TestInput(scope.ServiceProvider);
        }

        private sealed class TestInput
        {
            public TestInput(IServiceProvider sp)
            {
                Services = sp;
                Factory = sp.GetService<AppFactory>();
                Options = sp.GetService<FakeAppOptions>();
            }

            public AppFactory Factory { get; }
            public FakeAppOptions Options { get; }
            public IServiceProvider Services { get; }
        }
    }
}
