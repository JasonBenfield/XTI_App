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
            var services = await setup("/Fake/Current/Product/AddProduct");
            var app = await services.FakeApp();
            var adminRole = await app.Role(AppRoleName.Admin);
            await addRolesToUser(services, adminRole);
            var api = getApi(services);
            var hasAccess = await api.Product.AddProduct.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserDoesNoBelongToAnAllowedRole()
        {
            var services = await setup("/Fake/Current/Employee/AddEmployee");
            var app = await services.FakeApp();
            var viewerRole = await app.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(services, viewerRole);
            var api = getApi(services);
            var hasAccess = await api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserBelongsToADeniedRole()
        {
            var services = await setup("/Fake/Current/Product/AddProduct");
            var app = await services.FakeApp();
            var viewerRole = await app.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(services, viewerRole);
            var api = getApi(services);
            var hasAccess = await api.Product.AddProduct.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user belongs to a denied role");
        }

        [Test]
        public async Task ShouldHaveAccessToModifiedAction_WhenUserBelongsToAnAllowedRoleForTheDefaultModifier()
        {
            var services = await setup("/Fake/Current/Employee/AddEmployee", "IT");
            var user = await retrieveCurrentUser(services);
            var app = await services.FakeApp();
            var adminRole = await app.Role(AppRoleName.Admin);
            await user.AddRole(adminRole);
            var api = getApi(services);
            var hasAccess = await api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldNotHaveAccessToModifiedAction_WhenUserDoesNotBelongsToAnAllowedRoleForTheModifier_IfTheyBelongToAnAllowedRoleForTheDefaultModifier()
        {
            var services = await setup("/Fake/Current/Employee/AddEmployee", "IT");
            var user = await retrieveCurrentUser(services);
            var app = await services.FakeApp();
            var adminRole = await app.Role(AppRoleName.Admin);
            await user.AddRole(adminRole);
            var modCategory = await app.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier("IT");
            var viewerRole = await app.Role(FakeAppRoles.Instance.Viewer);
            await user.AddRole(viewerRole, modifier);
            var api = getApi(services);
            var hasAccess = await api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole_AndUserHasAccessToTheModifiedAction()
        {
            var services = await setup("/Fake/Current/Employee/AddEmployee", "IT");
            var user = await retrieveCurrentUser(services);
            var app = await services.FakeApp();
            var modCategory = await app.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier("IT");
            var adminRole = await app.Role(AppRoleName.Admin);
            await user.AddRole(adminRole, modifier);
            var api = getApi(services);
            var hasAccess = await api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserDoesNotBelongToAnAllowedRole_AndUserHasAccessToTheModifiedAction()
        {
            var services = await setup("/Fake/Current/Employee/AddEmployee", "IT");
            var user = await retrieveCurrentUser(services);
            var app = await services.FakeApp();
            var modCategory = await app.ModCategory(new ModifierCategoryName("Department"));
            var viewerRole = await app.Role(FakeAppRoles.Instance.Viewer);
            var modifier = await modCategory.Modifier("IT");
            await user.AddRole(viewerRole, modifier);
            var api = getApi(services);
            var hasAccess = await api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role even if they have access to the modified action");
        }

        [Test]
        public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole_AndTheActionHasTheDefaultModifier()
        {
            var services = await setup("/Fake/Current/Employee/AddEmployee");
            var app = await services.FakeApp();
            var adminRole = await app.Role(AppRoleName.Admin);
            await addRolesToUser(services, adminRole);
            var api = getApi(services);
            var hasAccess = await api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role and the action has the default modifier");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserBelongsToAnAllowedRole_ButDoesNotHaveAccessToTheModifiedAction()
        {
            var services = await setup("/Fake/Current/Employee/AddEmployee", "HR");
            var user = await retrieveCurrentUser(services);
            var app = await services.FakeApp();
            var adminRole = await app.Role(AppRoleName.Admin);
            var modCategory = await app.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier("IT");
            await user.AddRole(adminRole, modifier);
            var api = getApi(services);
            var hasAccess = await api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role for modified action");
        }

        [Test]
        public async Task AnonShouldNotHaveAccess_WhenAnonIsNotAllowed()
        {
            var services = await setup("/Fake/Current/Home/DoSomething");
            var factory = getAppFactory(services);
            var anonUser = await factory.Users().User(AppUserName.Anon);
            var userContext = getUserContext(services);
            userContext.SetUser(anonUser);
            var api = getApi(services);
            var hasAccess = await api.Home.DoSomething.HasAccess();
            Assert.That(hasAccess, Is.False, "Anon should not have access unless anons are allowed");
        }

        [Test]
        public async Task AnonShouldHaveAccess_WhenAnonIsAllowed()
        {
            var services = await setup("/Fake/Current/Login/Authenticate");
            var factory = getAppFactory(services);
            var anonUser = await factory.Users().User(AppUserName.Anon);
            var userContext = getUserContext(services);
            userContext.SetUser(anonUser);
            var api = getApi(services);
            var hasAccess = await api.Login.Authenticate.HasAccess();
            Assert.That(hasAccess, Is.True, "Anon should have access when anons are allowed");
        }

        [Test]
        public async Task ShouldAllowAccess_WhenTheUserIsAuthenticatedAndTheResourceAllowsAuthenticatedUsers()
        {
            var services = await setup("/Fake/Current/Home");
            var api = getApi(services);
            var hasAccess = await api.Home.HasAccess();
            Assert.That(hasAccess, Is.True, "User should have access to app when they are authenticated and the resource allows authenticated users");
        }

        [Test]
        public async Task ShouldNotAllowAccess_WhenTheUserIsNotAuthenticatedAndTheResourceAllowsAuthenticatedUsers()
        {
            var services = await setup("/Fake/Current/Home");
            var factory = getAppFactory(services);
            var anonUser = await factory.Users().User(AppUserName.Anon);
            var userContext = getUserContext(services);
            userContext.SetUser(anonUser);
            var api = getApi(services);
            var hasAccess = await api.Home.HasAccess();
            Assert.That(hasAccess, Is.False, "User should not have access to app when they are not authenticated and the resource allows authenticated users");
        }

        [Test]
        public async Task ShouldNotAllowAccess_WhenTheUserHasTheDenyAccessRole()
        {
            var services = await setup("/Fake/Current/Employee/AddEmployee", "IT");
            var user = await retrieveCurrentUser(services);
            var app = await services.FakeApp();
            var adminRole = await app.Role(AppRoleName.Admin);
            await user.AddRole(adminRole);
            var denyAccessRole = await app.Role(AppRoleName.DenyAccess);
            var modCategory = await app.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier("IT");
            await user.AddRole(adminRole, modifier);
            await user.AddRole(denyAccessRole, modifier);
            var api = getApi(services);
            var hasAccess = await api.Employee.AddEmployee.HasAccess();
            Assert.That(hasAccess, Is.False, "User should not have access when user has the deny access role");
        }

        private async Task addRolesToUser(IServiceProvider services, params AppRole[] roles)
        {
            var user = await retrieveCurrentUser(services);
            foreach (var role in roles)
            {
                await user.AddRole(role);
            }
        }

        private async Task<IServiceProvider> setup(string path, string department = "")
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    services =>
                    {
                        services.AddServicesForTests();
                        services.AddScoped<IAppContext, DefaultAppContext>();
                        services.AddScoped<IUserContext, FakeUserContext>();
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var factory = sp.GetService<AppFactory>();
            var clock = (FakeClock)sp.GetService<Clock>();
            await sp.Setup();
            var pathAccessor = (FakeXtiPathAccessor)sp.GetService<IXtiPathAccessor>();
            if (string.IsNullOrWhiteSpace(department))
            {
                pathAccessor.SetPath(XtiPath.Parse(path));
            }
            else
            {
                var app = await sp.FakeApp();
                var modCategory = await app.ModCategory(new ModifierCategoryName("Department"));
                var modifier = await modCategory.Modifier(department);
                pathAccessor.SetPath(XtiPath.Parse($"{path}/{modifier.ModKey().Value}"));
            }
            var user = await factory.Users().Add
            (
                new AppUserName("someone"), new FakeHashedPassword("Password"), clock.Now()
            );
            var userContext = getUserContext(sp);
            userContext.SetUser(user);
            return sp;
        }

        private AppFactory getAppFactory(IServiceProvider sp) => sp.GetService<AppFactory>();

        private async Task<AppUser> retrieveCurrentUser(IServiceProvider sp)
        {
            var userContext = getUserContext(sp);
            return (AppUser)await userContext.User();
        }

        private FakeUserContext getUserContext(IServiceProvider sp)
            => (FakeUserContext)sp.GetService<IUserContext>();

        private FakeAppApi getApi(IServiceProvider sp) => (FakeAppApi)sp.GetService<IAppApi>();

    }
}
