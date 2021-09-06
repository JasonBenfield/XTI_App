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
using XTI_App.TestFakes;
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
        public async Task ShouldRetrieveUserFromCache()
        {
            var services = await setup();
            var userContext = getUserContext(services);
            var originalUser = await userContext.User();
            var originalUserName = originalUser.UserName();
            var db = getMainDbContext(services);
            var sourceUser = await testUser(services);
            var userRecord = await db.Users.Retrieve().FirstAsync(u => u.ID == sourceUser.ID.Value);
            await db.Users.Update(userRecord, r => r.UserName = "changed.user");
            var cachedUser = await userContext.User();
            Assert.That(cachedUser.UserName, Is.EqualTo(originalUserName), "Should retrieve user from cache");
        }

        [Test]
        public async Task ShouldRefreshUser()
        {
            var services = await setup();
            var userContext = getUserContext(services);
            var user = await userContext.User();
            var app = await services.FakeApp();
            var modifier = await app.DefaultModifier();
            await user.Roles(modifier);
            var adminRole = await app.Role(AppRoleName.Admin);
            var sourceUser = await testUser(services);
            await sourceUser.AddRole(adminRole);
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
            var app = await services.FakeApp();
            var modifier = await app.DefaultModifier();
            var userRoles = await user.Roles(modifier);
            var viewerRole = await app.Role(FakeAppRoles.Instance.Viewer);
            Assert.That(userRoles.Select(ur => ur.ID), Is.EquivalentTo(new[] { viewerRole.ID }), "Should retrieve user roles from source");
        }

        [Test]
        public async Task ShouldRetrieveUserRolesFromCache()
        {
            var services = await setup();
            var userContext = getUserContext(services);
            var user = await userContext.User();
            var app = await services.FakeApp();
            var modifier = await app.DefaultModifier();
            var userRoles = await user.Roles(modifier);
            var adminRole = await app.Role(AppRoleName.Admin);
            var sourceUser = await testUser(services);
            await sourceUser.AddRole(adminRole);
            var cachedUser = await userContext.User();
            var cachedUserRoles = await cachedUser.Roles(modifier);
            var viewerRole = await app.Role(FakeAppRoles.Instance.Viewer);
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
            var appFactory = sp.GetService<AppFactory>();
            var clock = sp.GetService<Clock>();
            var user = await appFactory.Users().Add
            (
                new AppUserName("test.user"),
                new FakeHashedPassword("Testing12345"),
                clock.Now()
            );
            var viewerRole = await fakeSetup.App.Role(FakeAppRoles.Instance.Viewer);
            await user.AddRole(viewerRole);
            var userContext = (FakeUserContext)sp.GetService<ISourceUserContext>();
            userContext.SetUser(user);
            return sp;
        }

        private CachedUserContext getUserContext(IServiceProvider sp)
            => (CachedUserContext)sp.GetService<IUserContext>();

        private MainDbContext getMainDbContext(IServiceProvider sp) => sp.GetService<MainDbContext>();

        private Task<AppUser> testUser(IServiceProvider sp)
        {
            var factory = getAppFactory(sp);
            return factory.Users().User(new AppUserName("test.user"));
        }

        private AppFactory getAppFactory(IServiceProvider sp) => sp.GetService<AppFactory>();

    }
}
