using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.EfApi;
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
            var input = await setup("/Fake/Current/Product/AddProduct");
            var adminRole = await input.App.Role(AppRoleName.Admin);
            await addRolesToUser(input, adminRole);
            var hasAccess = await input.Api.Product.AddProduct.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserDoesNoBelongToAnAllowedRole()
        {
            var input = await setup("/Fake/Current/Employee/AddEmployee");
            var viewerRole = await input.App.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(input, viewerRole);
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserBelongsToADeniedRole()
        {
            var input = await setup("/Fake/Current/Product/AddProduct");
            var viewerRole = await input.App.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(input, viewerRole);
            var hasAccess = await input.Api.Product.AddProduct.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user belongs to a denied role");
        }

        [Test]
        public async Task ShouldHaveAccessToModifiedAction_WhenUserBelongsToAnAllowedRoleForTheDefaultModifier()
        {
            var input = await setup("/Fake/Current/Employee/AddEmployee", "IT");
            var user = await input.User();
            var adminRole = await input.App.Role(AppRoleName.Admin);
            await user.AddRole(adminRole);
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldNotHaveAccessToModifiedAction_WhenUserDoesNotBelongsToAnAllowedRoleForTheModifier_IfTheyBelongToAnAllowedRoleForTheDefaultModifier()
        {
            var input = await setup("/Fake/Current/Employee/AddEmployee", "IT");
            var user = await input.User();
            var adminRole = await input.App.Role(AppRoleName.Admin);
            await user.AddRole(adminRole);
            var modCategory = await input.App.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier("IT");
            var viewerRole = await input.App.Role(FakeAppRoles.Instance.Viewer);
            await user.AddRole(viewerRole, modifier);
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole_AndUserHasAccessToTheModifiedAction()
        {
            var input = await setup("/Fake/Current/Employee/AddEmployee", "IT");
            var user = await input.User();
            var modCategory = await input.App.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier("IT");
            var adminRole = await input.App.Role(AppRoleName.Admin);
            await user.AddRole(adminRole, modifier);
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserDoesNotBelongToAnAllowedRole_AndUserHasAccessToTheModifiedAction()
        {
            var input = await setup("/Fake/Current/Employee/AddEmployee", "IT");
            var user = await input.User();
            var modCategory = await input.App.ModCategory(new ModifierCategoryName("Department"));
            var viewerRole = await input.App.Role(FakeAppRoles.Instance.Viewer);
            var modifier = await modCategory.Modifier("IT");
            await user.AddRole(viewerRole, modifier);
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role even if they have access to the modified action");
        }

        [Test]
        public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole_AndTheActionHasTheDefaultModifier()
        {
            var input = await setup("/Fake/Current/Employee/AddEmployee");
            var adminRole = await input.App.Role(AppRoleName.Admin);
            await addRolesToUser(input, adminRole);
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role and the action has the default modifier");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserBelongsToAnAllowedRole_ButDoesNotHaveAccessToTheModifiedAction()
        {
            var input = await setup("/Fake/Current/Employee/AddEmployee", "HR");
            var user = await input.User();
            var adminRole = await input.App.Role(AppRoleName.Admin);
            var modCategory = await input.App.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier("IT");
            await user.AddRole(adminRole, modifier);
            var hasAccess = await input.Api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role for modified action");
        }

        [Test]
        public async Task AnonShouldNotHaveAccess_WhenAnonIsNotAllowed()
        {
            var input = await setup("/Fake/Current/Home/DoSomething");
            var anonUser = await input.Factory.Users().User(AppUserName.Anon);
            input.UserContext.SetUser(anonUser);
            var hasAccess = await input.Api.Home.DoSomething.HasAccess();
            Assert.That(hasAccess, Is.False, "Anon should not have access unless anons are allowed");
        }

        [Test]
        public async Task AnonShouldHaveAccess_WhenAnonIsAllowed()
        {
            var input = await setup("/Fake/Current/Login/Authenticate");
            var anonUser = await input.Factory.Users().User(AppUserName.Anon);
            input.UserContext.SetUser(anonUser);
            var hasAccess = await input.Api.Login.Authenticate.HasAccess();
            Assert.That(hasAccess, Is.True, "Anon should have access when anons are allowed");
        }

        [Test]
        public async Task ShouldAllowAccess_WhenTheUserIsAuthenticatedAndTheResourceAllowsAuthenticatedUsers()
        {
            var input = await setup("/Fake/Current/Home");
            var hasAccess = await input.Api.Home.HasAccess();
            Assert.That(hasAccess, Is.True, "User should have access to app when they are authenticated and the resource allows authenticated users");
        }

        [Test]
        public async Task ShouldNotAllowAccess_WhenTheUserIsNotAuthenticatedAndTheResourceAllowsAuthenticatedUsers()
        {
            var input = await setup("/Fake/Current/Home");
            var anonUser = await input.Factory.Users().User(AppUserName.Anon);
            input.UserContext.SetUser(anonUser);
            var hasAccess = await input.Api.Home.HasAccess();
            Assert.That(hasAccess, Is.False, "User should not have access to app when they are not authenticated and the resource allows authenticated users");
        }

        private async Task addRolesToUser(TestInput input, params AppRole[] roles)
        {
            var user = await input.User();
            foreach (var role in roles)
            {
                await user.AddRole(role);
            }
        }

        private async Task<TestInput> setup(string path, string department = "")
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    services =>
                    {
                        services.AddServicesForTests();
                        services.AddScoped<XtiPathAccessor>();
                        services.AddScoped(sp =>
                        {
                            var accessor = sp.GetService<XtiPathAccessor>();
                            return accessor.Path;
                        });
                        services.AddScoped<IAppContext, DefaultAppContext>();
                        services.AddScoped<IUserContext, FakeUserContext>();
                        services.AddScoped<IAppApiUser, AppApiUser>();
                        services.AddSingleton(sp => FakeInfo.AppKey);
                        services.AddSingleton(_ => AppVersionKey.Current);
                        services.AddScoped<FakeAppApi>();
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var factory = sp.GetService<AppFactory>();
            var clock = (FakeClock)sp.GetService<Clock>();
            var appSetup = new FakeAppSetup(factory, clock);
            await appSetup.Run(AppVersionKey.Current);
            if (string.IsNullOrWhiteSpace(department))
            {
                var pathAccessor = sp.GetService<XtiPathAccessor>();
                pathAccessor.Path = XtiPath.Parse(path);
            }
            else
            {
                var modCategory = await appSetup.App.ModCategory(new ModifierCategoryName("Department"));
                var modifier = await modCategory.Modifier(department);
                var pathAccessor = sp.GetService<XtiPathAccessor>();
                pathAccessor.Path = XtiPath.Parse($"{path}/{modifier.ModKey().Value}");
            }
            var user = await factory.Users().Add
            (
                new AppUserName("someone"), new FakeHashedPassword("Password"), clock.Now()
            );
            var input = new TestInput(sp, appSetup.App);
            input.UserContext.SetUser(user);
            return input;
        }

        private sealed class XtiPathAccessor
        {
            public XtiPath Path { get; set; }
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
