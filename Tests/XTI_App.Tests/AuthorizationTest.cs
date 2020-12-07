using MainDB.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_App.TestFakes;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.Tests
{
    public sealed class AuthorizationTest
    {
        [Test]
        public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole()
        {
            var input = await setup();
            var adminRole = await input.App.Role(FakeAppRoles.Instance.Admin);
            await addRolesToUser(input, adminRole);
            var hasAccess = await input.Api.Product.AddProduct.HasAccess(ModifierKey.Default);
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserDoesNoBelongToAnAllowedRole()
        {
            var input = await setup();
            var viewerRole = await input.App.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(input, viewerRole);
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess(ModifierKey.Default);
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserBelongsToADeniedRole()
        {
            var input = await setup();
            var viewerRole = await input.App.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(input, viewerRole);
            var hasAccess = await input.Api.Product.AddProduct.HasAccess(ModifierKey.Default);
            Assert.That(hasAccess, Is.False, "Should not have access when user belongs to a denied role");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserBelongsToADeniedRoleEvenIfTheyBelongToAnAllowedRole()
        {
            var input = await setup();
            var adminRole = await input.App.Role(FakeAppRoles.Instance.Admin);
            var viewerRole = await input.App.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(input, adminRole, viewerRole);
            var hasAccess = await input.Api.Product.AddProduct.HasAccess(ModifierKey.Default);
            Assert.That(hasAccess, Is.False, "Should not have access when user belongs to a denied role even if they belong to an allowed role");
        }

        [Test]
        public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole_AndUserHasAccessToTheModifiedAction()
        {
            var input = await setup();
            var viewerRole = await input.App.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(input, viewerRole);
            var user = await input.User();
            var modCategory = await input.App.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier(new ModifierKey("IT"));
            await user.AddModifier(modifier);
            var adminRole = await input.App.Role(FakeAppRoles.Instance.Admin);
            var modifierKey = new ModifierKey("IT");
            await addRolesToUser(input, adminRole);
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess(modifierKey);
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole_AndTheActionHasTheDefaultModifier()
        {
            var input = await setup();
            var adminRole = await input.App.Role(FakeAppRoles.Instance.Admin);
            await addRolesToUser(input, adminRole);
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess(ModifierKey.Default);
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role and the action has the default modifier");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserBelongsToAnAllowedRole_ButDoesNotHaveAccessToTheModifiedAction()
        {
            var input = await setup();
            var adminRole = await input.App.Role(FakeAppRoles.Instance.Admin);
            await addRolesToUser(input, adminRole);
            var user = await input.User();
            var modCategory = await input.App.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier(new ModifierKey("IT"));
            await user.AddModifier(modifier);
            var modifierKey = new ModifierKey("HR");
            var viewerRole = await input.App.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(input, viewerRole);
            var hasAccess = await input.Api.Product.AddProduct.HasAccess(modifierKey);
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldHaveAccess_WhenUserHasFullAccessToTheModCategory()
        {
            var input = await setup();
            var adminRole = await input.App.Role(FakeAppRoles.Instance.Admin);
            await addRolesToUser(input, adminRole);
            var category = await input.App.ModCategory(new ModifierCategoryName("Department"));
            var user = await input.User();
            await user.GrantFullAccessToModCategory(category);
            var modifier = new ModifierKey("IT");
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess(modifier);
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role for modified action");
        }

        [Test]
        public async Task AnonShouldNotHaveAccess_WhenAnonIsNotAllowed()
        {
            var input = await setup();
            var anonUser = await input.Factory.Users().User(AppUserName.Anon);
            input.UserContext.SetUser(anonUser);
            var hasAccess = await input.Api.Home.DoSomething.HasAccess(ModifierKey.Default);
            Assert.That(hasAccess, Is.False, "Anon should not have access unless anons are allowed");
        }

        [Test]
        public async Task AnonShouldHaveAccess_WhenAnonIsAllowed()
        {
            var input = await setup();
            var anonUser = await input.Factory.Users().User(AppUserName.Anon);
            input.UserContext.SetUser(anonUser);
            var hasAccess = await input.Api.Login.Authenticate.HasAccess(ModifierKey.Default);
            Assert.That(hasAccess, Is.True, "Anon should have access when anons are allowed");
        }

        [Test]
        public async Task ShouldAllowAccess_WhenTheUserIsAuthenticatedAndTheResourceAllowsAuthenticatedUsers()
        {
            var input = await setup();
            var hasAccess = await input.Api.Home.HasAccess(ModifierKey.Default);
            Assert.That(hasAccess, Is.True, "User should have access to app when they are authenticated and the resource allows authenticated users");
        }

        [Test]
        public async Task ShouldNotAllowAccess_WhenTheUserIsNotAuthenticatedAndTheResourceAllowsAuthenticatedUsers()
        {
            var input = await setup();
            var anonUser = await input.Factory.Users().User(AppUserName.Anon);
            input.UserContext.SetUser(anonUser);
            var hasAccess = await input.Api.Home.HasAccess(ModifierKey.Default);
            Assert.That(hasAccess, Is.False, "User should not have access to app when they are not authenticated and the resource allows authenticated users");
        }

        [Test]
        public async Task ShouldHaveAccessToApp_WhenTheUserBelongsToAnyAppRoles()
        {
            var input = await setup();
            var viewerRole = await input.App.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(input, viewerRole);
            var hasAccess = await input.Api.HasAccess();
            Assert.That(hasAccess, Is.True, "User should have access to app when they belong to any app roles");
        }

        [Test]
        public async Task ShouldNotHaveAccessToApp_WhenTheUserDoesNotBelongToAnyAppRoles()
        {
            var input = await setup();
            var hasAccess = await input.Api.HasAccess();
            Assert.That(hasAccess, Is.False, "User should not have access to app when they do not belong to any app roles");
        }

        private async Task addRolesToUser(TestInput input, params AppRole[] roles)
        {
            var user = await input.User();
            foreach (var role in roles)
            {
                await user.AddRole(role);
            }
        }

        private async Task<TestInput> setup()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    services =>
                    {
                        services.AddAppDbContextForInMemory();
                        services.AddScoped<AppFactory>();
                        services.AddSingleton(sp => FakeAppKey.AppKey);
                        services.AddScoped<IAppContext, DefaultAppContext>();
                        services.AddScoped<IUserContext, FakeUserContext>();
                        services.AddScoped<IAppApiUser, XtiAppApiUser>();
                        services.AddSingleton<Clock, FakeClock>();
                        services.AddSingleton(sp => FakeAppKey.AppKey);
                        services.AddScoped(sp =>
                        {
                            var appKey = sp.GetService<AppKey>();
                            var apiUser = sp.GetService<IAppApiUser>();
                            return new FakeAppApi(appKey, apiUser, AppVersionKey.Current);
                        });
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var factory = sp.GetService<AppFactory>();
            var clock = (FakeClock)sp.GetService<Clock>();
            var appSetup = new FakeAppSetup(factory, clock);
            await appSetup.Run();
            var user = await factory.Users().Add
            (
                new AppUserName("someone"), new FakeHashedPassword("Password"), clock.Now()
            );
            var input = new TestInput(sp, appSetup.App);
            input.UserContext.SetUser(user);
            return input;
        }

        private sealed class TestInput
        {
            public TestInput(IServiceProvider sp, App app)
            {
                Factory = sp.GetService<AppFactory>();
                Clock = sp.GetService<FakeClock>();
                UserContext = (FakeUserContext)sp.GetService<IUserContext>();
                App = app;
                Api = sp.GetService<FakeAppApi>();
            }

            public AppFactory Factory { get; }
            public FakeClock Clock { get; }
            public App App { get; }
            public FakeUserContext UserContext { get; }
            public FakeAppApi Api { get; }

            public async Task<AppUser> User() => (AppUser)await UserContext.User();
        }
    }
}
