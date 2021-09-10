using MainDB.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_App.Fakes;
using XTI_App.Fakes;
using XTI_Configuration.Extensions;
using XTI_Core;

namespace XTI_App.Tests
{
    public sealed class CachedUserContextTest
    {
        [Test]
        public async Task ShouldRetrieveUserFromSource()
        {
            var services = await setup();
            var userContext = getUserContext(services);
            var userFromContext = await userContext.User();
            var sourceUser = await testUser(services);
            Assert.That(userFromContext.ID, Is.EqualTo(sourceUser.ID), "Should retrieve user from source");
            Assert.That(userFromContext.UserName(), Is.EqualTo(sourceUser.UserName()), "Should retrieve user from source");
        }

        [Test]
        public async Task ShouldRefreshUser()
        {
            var services = await setup();
            var userContext = getUserContext(services);
            var user = await userContext.User();
            var appSetup = services.GetService<FakeAppSetup>();
            var modCategory = await appSetup.App.ModCategory(ModifierCategoryName.Default);
            var modifier = await modCategory.Modifier(ModifierKey.Default);
            await user.Roles(modifier);
            var adminRole = (FakeAppRole)await appSetup.App.Role(AppRoleName.Admin);
            var sourceUser = await testUser(services);
            sourceUser.AddRoles(modifier, adminRole);
            userContext.ClearCache(user.UserName());
            var userRoles = await user.Roles(modifier);
            Assert.That(userRoles.Select(r => r.Name()), Has.One.EqualTo(AppRoleName.Admin), "Should clear cache");
        }

        [Test]
        public async Task ShouldRetrieveUserRolesFromSource()
        {
            var services = await setup();
            var userContext = getUserContext(services);
            var user = await userContext.User();
            var appSetup = services.GetService<FakeAppSetup>();
            var modCategory = await appSetup.App.ModCategory(ModifierCategoryName.Default);
            var modifier = await modCategory.Modifier(ModifierKey.Default);
            var userRoles = await user.Roles(modifier);
            var viewerRole = await appSetup.App.Role(FakeAppRoles.Instance.Viewer);
            Assert.That(userRoles.Select(ur => ur.ID), Is.EquivalentTo(new[] { viewerRole.ID }), "Should retrieve user roles from source");
        }

        [Test]
        public async Task ShouldRetrieveUserRolesFromCache()
        {
            var services = await setup();
            var userContext = getUserContext(services);
            var user = await userContext.User();
            var appSetup = services.GetService<FakeAppSetup>();
            var modCategory = await appSetup.App.ModCategory(ModifierCategoryName.Default);
            var modifier = await modCategory.Modifier(ModifierKey.Default);
            var userRoles = await user.Roles(modifier);
            var adminRole = (FakeAppRole)await appSetup.App.Role(AppRoleName.Admin);
            var sourceUser = await testUser(services);
            sourceUser.AddRoles(modifier, adminRole);
            var cachedUser = await userContext.User();
            var cachedUserRoles = await cachedUser.Roles(modifier);
            var viewerRole = await appSetup.App.Role(FakeAppRoles.Instance.Viewer);
            Assert.That(userRoles.Select(ur => ur.ID), Is.EquivalentTo(new[] { viewerRole.ID }), "Should retrieve user roles from source");
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
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var xtiPathAccessor = (FakeXtiPathAccessor)sp.GetService<IXtiPathAccessor>();
            xtiPathAccessor.SetPath(XtiPath.Parse("/Fake/Current/Employees/Index"));
            var fakeSetup = sp.GetService<FakeAppSetup>();
            await fakeSetup.Run(AppVersionKey.Current);
            var modCategory = await fakeSetup.App.ModCategory(ModifierCategoryName.Default);
            var modifier = await modCategory.Modifier(ModifierKey.Default);
            var userContext = sp.GetService<FakeUserContext>();
            var userName = new AppUserName("test.user");
            var user = userContext.AddUser(userName);
            var viewerRole = (FakeAppRole)await fakeSetup.App.Role(FakeAppRoles.Instance.Viewer);
            user.AddRoles(modifier, viewerRole);
            userContext.SetCurrentUser(userName);
            return sp;
        }

        private CachedUserContext getUserContext(IServiceProvider sp)
            => (CachedUserContext)sp.GetService<IUserContext>();

        private MainDbContext getMainDbContext(IServiceProvider sp) => sp.GetService<MainDbContext>();

        private async Task<FakeAppUser> testUser(IServiceProvider sp)
        {
            var userContext = sp.GetService<FakeUserContext>();
            var user = await userContext.User();
            return (FakeAppUser)user;
        }

    }
}
