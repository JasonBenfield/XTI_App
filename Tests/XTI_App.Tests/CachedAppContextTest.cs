using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_App.Fakes;
using XTI_Configuration.Extensions;

namespace XTI_App.Tests
{
    public sealed class CachedAppContextTest
    {
        [Test]
        public async Task ShouldRetrieveAppFromSource()
        {
            var services = await setup();
            var appContext = getAppContext(services);
            var appFromContext = await appContext.App();
            var appSetup = services.GetService<FakeAppSetup>();
            Assert.That(appFromContext.ID, Is.EqualTo(appSetup.App.ID), "Should retrieve app from source");
            Assert.That(appFromContext.Title, Is.EqualTo(appSetup.App.Title), "Should retrieve app from source");
        }

        [Test]
        public async Task ShouldRetrieveAppFromCache()
        {
            var services = await setup();
            var appContext = getAppContext(services);
            var appFromContext = await appContext.App();
            var originalTitle = appFromContext.Title;
            var appSetup = services.GetService<FakeAppSetup>();
            appSetup.App.SetTitle("New title");
            var cachedApp = await appContext.App();
            Assert.That(cachedApp.Title, Is.EqualTo(originalTitle), "Should retrieve app from cache");
        }

        [Test]
        public async Task ShouldRetrieveAppRolesFromSource()
        {
            var services = await setup();
            var appContext = getAppContext(services);
            var app = await appContext.App();
            var appRoles = await app.Roles();
            var expectedRoleNames = new[]
            {
                FakeInfo.Roles.Viewer,
                FakeInfo.Roles.Manager
            }
            .Union(AppRoleName.DefaultRoles());
            Assert.That
            (
                appRoles.Select(ar => ar.Name()),
                Is.EquivalentTo(expectedRoleNames),
                "Should retrieve app roles from source"
            );
        }

        [Test]
        public async Task ShouldRetrieveAppRolesFromCache()
        {
            var services = await setup();
            var appContext = getAppContext(services);
            var originalApp = await appContext.App();
            var originalAppRoles = await originalApp.Roles();
            var appSetup = services.GetService<FakeAppSetup>();
            appSetup.App.AddRole(new AppRoleName("New Role"));
            var cachedApp = await appContext.App();
            var cachedAppRoles = await cachedApp.Roles();
            var expectedRoleNames = new[]
                {
                    FakeInfo.Roles.Viewer,
                    FakeInfo.Roles.Manager
                }
                .Union(AppRoleName.DefaultRoles());
            Assert.That
            (
                cachedAppRoles.Select(ar => ar.Name()),
                Is.EquivalentTo(expectedRoleNames),
                "Should retrieve app roles from source"
            );
        }

        [Test]
        public async Task ShouldRetrieveAppVersionFromCache()
        {
            var services = await setup();
            var appContext = getAppContext(services);
            var originalApp = await appContext.App();
            var originalVersion = await originalApp.Version(AppVersionKey.Current);
            var appSetup = services.GetService<FakeAppSetup>();
            var newVersion = appSetup.App.AddVersion(new AppVersionKey(1));
            var cachedApp = await appContext.App();
            var cachedVersion = await cachedApp.Version(AppVersionKey.Current);
            Assert.That(cachedVersion.ID, Is.EqualTo(originalVersion.ID), "Should retrieve current version from cache");
        }

        [Test]
        public async Task ShouldRetrieveResourceGroupFromSource()
        {
            var services = await setup();
            var appContext = getAppContext(services);
            var app = await appContext.App();
            var version = await app.Version(AppVersionKey.Current);
            var resourceGroupName = new ResourceGroupName("Employee");
            var resourceGroup = await version.ResourceGroup(resourceGroupName);
            Assert.That(resourceGroup.Name(), Is.EqualTo(resourceGroupName), "Should retrieve resource group from source");
        }

        [Test]
        public async Task ShouldRetrieveModifierCategoryFromSource()
        {
            var services = await setup();
            var appContext = getAppContext(services);
            var app = await appContext.App();
            var version = await app.Version(AppVersionKey.Current);
            var resourceGroup = await version.ResourceGroup(new ResourceGroupName("Employee"));
            var modCategory = await resourceGroup.ModCategory();
            Assert.That(modCategory.Name(), Is.EqualTo(new ModifierCategoryName("Department")), "Should retrieve modifier category from source");
        }

        [Test]
        public async Task ShouldRetrieveModifierCategoryFromCache()
        {
            var services = await setup();
            var appContext = getAppContext(services);
            var appFromContext = await appContext.App();
            var version = await appFromContext.Version(AppVersionKey.Current);
            var resourceGroup = await version.ResourceGroup(new ResourceGroupName("Employee"));
            await resourceGroup.ModCategory();
            var appSetup = services.GetService<FakeAppSetup>();
            var currentVersion = await appSetup.App.Version(AppVersionKey.Current);
            var sourceResourceGroup = await currentVersion.ResourceGroup(new ResourceGroupName("Employee"));
            var cachedModCategory = await resourceGroup.ModCategory();
            Assert.That(cachedModCategory.Name(), Is.EqualTo(new ModifierCategoryName("Department")), "Should retrieve modifier category from source");
        }

        [Test]
        public async Task ShouldRetrieveResourceFromSource()
        {
            var services = await setup();
            var appContext = getAppContext(services);
            var app = await appContext.App();
            var version = await app.Version(AppVersionKey.Current);
            var resourceGroup = await version.ResourceGroup(new ResourceGroupName("Employee"));
            var resource = await resourceGroup.Resource(new ResourceName("AddEmployee"));
            Assert.That(resource.Name(), Is.EqualTo(new ResourceName("AddEmployee")), "Should retrieve resource from source");
        }

        private async Task<IServiceProvider> setup()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration
                (
                    (hostContext, config) =>
                    {
                        config.UseXtiConfiguration(hostContext.HostingEnvironment, new string[] { });
                    }
                )
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddServicesForTests();
                        services.AddScoped<FakeAppSetup>();
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var xtiPathAccessor = (FakeXtiPathAccessor)sp.GetService<IXtiPathAccessor>();
            xtiPathAccessor.SetPath(XtiPath.Parse("/Fake/Current/Employees/Index"));
            await sp.Setup();
            return sp;
        }

        private IAppContext getAppContext(IServiceProvider services) => services.GetService<IAppContext>();
    }
}
