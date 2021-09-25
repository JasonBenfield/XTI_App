using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Fakes;

namespace XTI_App.Tests
{
    public sealed class AuthorizationTest
    {
        [Test]
        public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole()
        {
            var services = await setup();
            var appSetup = services.GetService<FakeAppSetup>();
            var adminRole = await appSetup.App.Role(AppRoleName.Admin);
            await addRolesToUser(services, adminRole);
            var api = getApi(services);
            var action = api.Product.AddProduct;
            setPath(services, action);
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserDoesNoBelongToAnAllowedRole()
        {
            var services = await setup();
            var appSetup = services.GetService<FakeAppSetup>();
            var viewerRole = await appSetup.App.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(services, viewerRole);
            var api = getApi(services);
            var action = api.Employee.AddEmployee;
            setPath(services, action);
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserBelongsToADeniedRole()
        {
            var services = await setup();
            var appSetup = services.GetService<FakeAppSetup>();
            var viewerRole = await appSetup.App.Role(FakeAppRoles.Instance.Viewer);
            await addRolesToUser(services, viewerRole);
            var api = getApi(services);
            var action = api.Product.AddProduct;
            setPath(services, action, "IT");
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user belongs to a denied role");
        }

        [Test]
        public async Task ShouldHaveAccessToModifiedAction_WhenUserBelongsToAnAllowedRoleForTheDefaultModifier()
        {
            var services = await setup();
            var api = getApi(services);
            var user = await retrieveCurrentUser(services);
            var appSetup = services.GetService<FakeAppSetup>();
            var adminRole = await appSetup.App.Role(AppRoleName.Admin);
            await user.AddRole(adminRole);
            var action = api.Employee.AddEmployee;
            setPath(services, action, "IT");
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldNotHaveAccessToModifiedAction_WhenUserDoesNotBelongsToAnAllowedRoleForTheModifier_IfTheyBelongToAnAllowedRoleForTheDefaultModifier()
        {
            var services = await setup();
            var user = await retrieveCurrentUser(services);
            var appSetup = services.GetService<FakeAppSetup>();
            var adminRole = await appSetup.App.Role(AppRoleName.Admin);
            await user.AddRole(adminRole);
            var modCategory = await appSetup.App.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier(new ModifierKey("IT"));
            var viewerRole = await appSetup.App.Role(FakeAppRoles.Instance.Viewer);
            user.AddRoles(modifier, viewerRole);
            var api = getApi(services);
            var action = api.Employee.AddEmployee;
            setPath(services, action, "IT");
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole_AndUserHasAccessToTheModifiedAction()
        {
            var services = await setup();
            var user = await retrieveCurrentUser(services);
            var appSetup = services.GetService<FakeAppSetup>();
            var modCategory = await appSetup.App.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier(new ModifierKey("IT"));
            var adminRole = await appSetup.App.Role(AppRoleName.Admin);
            user.AddRoles(modifier, adminRole);
            var api = getApi(services);
            var action = api.Employee.AddEmployee;
            setPath(services, action, "IT");
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role for modified action");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserDoesNotBelongToAnAllowedRole_AndUserHasAccessToTheModifiedAction()
        {
            var services = await setup();
            var user = await retrieveCurrentUser(services);
            var appSetup = services.GetService<FakeAppSetup>();
            var modCategory = await appSetup.App.ModCategory(new ModifierCategoryName("Department"));
            var viewerRole = await appSetup.App.Role(FakeAppRoles.Instance.Viewer);
            var modifier = await modCategory.Modifier(new ModifierKey("IT"));
            user.AddRoles(modifier, viewerRole);
            var api = getApi(services);
            var action = api.Employee.AddEmployee;
            setPath(services, action, "IT");
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role even if they have access to the modified action");
        }

        [Test]
        public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole_AndTheActionHasTheDefaultModifier()
        {
            var services = await setup();
            var appSetup = services.GetService<FakeAppSetup>();
            var adminRole = await appSetup.App.Role(AppRoleName.Admin);
            await addRolesToUser(services, adminRole);
            var api = getApi(services);
            var action = api.Employee.AddEmployee;
            setPath(services, action);
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role and the action has the default modifier");
        }

        [Test]
        public async Task ShouldNotHaveAccess_WhenUserBelongsToAnAllowedRole_ButDoesNotHaveAccessToTheModifiedAction()
        {
            var services = await setup();
            var user = await retrieveCurrentUser(services);
            var appSetup = services.GetService<FakeAppSetup>();
            var adminRole = await appSetup.App.Role(AppRoleName.Admin);
            var modCategory = await appSetup.App.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier(new ModifierKey("IT"));
            user.AddRoles(modifier, adminRole);
            var api = getApi(services);
            var action = api.Employee.AddEmployee;
            setPath(services, action, "HR");
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.False, "Should not have access when user does not belong to an allowed role for modified action");
        }

        [Test]
        public async Task AnonShouldNotHaveAccess_WhenAnonIsNotAllowed()
        {
            var services = await setup();
            var userContext = getUserContext(services);
            userContext.SetCurrentUser(AppUserName.Anon);
            var api = getApi(services);
            var action = api.Home.DoSomething;
            setPath(services, action);
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.False, "Anon should not have access unless anons are allowed");
        }

        [Test]
        public async Task AnonShouldHaveAccess_WhenAnonIsAllowed()
        {
            var services = await setup();
            var userContext = getUserContext(services);
            userContext.SetCurrentUser(AppUserName.Anon);
            var api = getApi(services);
            var action = api.Login.Authenticate;
            setPath(services, action);
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.True, "Anon should have access when anons are allowed");
        }

        [Test]
        public async Task ShouldAllowAccess_WhenTheUserIsAuthenticatedAndTheResourceAllowsAuthenticatedUsers()
        {
            var services = await setup();
            var api = getApi(services);
            var group = api.Home;
            setPath(services, group);
            var hasAccess = await group.HasAccess();
            Assert.That(hasAccess, Is.True, "User should have access to app when they are authenticated and the resource allows authenticated users");
        }

        [Test]
        public async Task ShouldNotAllowAccess_WhenTheUserIsNotAuthenticatedAndTheResourceAllowsAuthenticatedUsers()
        {
            var services = await setup();
            var userContext = getUserContext(services);
            userContext.SetCurrentUser(AppUserName.Anon);
            var api = getApi(services);
            var group = api.Home;
            setPath(services, group);
            var hasAccess = await group.HasAccess();
            Assert.That(hasAccess, Is.False, "User should not have access to app when they are not authenticated and the resource allows authenticated users");
        }

        [Test]
        public async Task ShouldNotAllowAccess_WhenTheUserHasTheDenyAccessRole()
        {
            var services = await setup();
            var user = await retrieveCurrentUser(services);
            var appSetup = services.GetService<FakeAppSetup>();
            var adminRole = await appSetup.App.Role(AppRoleName.Admin);
            await user.AddRole(adminRole);
            var denyAccessRole = await appSetup.App.Role(AppRoleName.DenyAccess);
            var modCategory = await appSetup.App.ModCategory(new ModifierCategoryName("Department"));
            var modifier = await modCategory.Modifier(new ModifierKey("IT"));
            user.AddRoles(modifier, adminRole, denyAccessRole);
            var api = getApi(services);
            var action = api.Employee.AddEmployee;
            setPath(services, action, "IT");
            var hasAccess = await action.HasAccess();
            Assert.That(hasAccess, Is.False, "User should not have access when user has the deny access role");
        }

        private async Task addRolesToUser(IServiceProvider services, params FakeAppRole[] roles)
        {
            var user = await retrieveCurrentUser(services);
            foreach (var role in roles)
            {
                await user.AddRole(role);
            }
        }

        private async Task<IServiceProvider> setup()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddServicesForTests(hostContext.Configuration);
                        services.AddScoped<IAppContext>(sp => sp.GetService<FakeAppContext>());
                        services.AddScoped<IUserContext>(sp => sp.GetService<FakeUserContext>());
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            await sp.Setup();
            var userContext = (FakeUserContext)sp.GetService<ISourceUserContext>();
            var userName = new AppUserName("someone");
            userContext.AddUser(userName);
            userContext.SetCurrentUser(userName);
            return sp;
        }

        private void setPath(IServiceProvider sp, IAppApiGroup group)
        {
            var pathAccessor = (FakeXtiPathAccessor)sp.GetService<IXtiPathAccessor>();
            pathAccessor.SetPath(group.Path);
        }

        private void setPath<TModel, TResult>(IServiceProvider sp, AppApiAction<TModel, TResult> action, string department = "")
        {
            var pathAccessor = (FakeXtiPathAccessor)sp.GetService<IXtiPathAccessor>();
            pathAccessor.SetPath(action.Path.WithModifier(new ModifierKey(department)));
        }

        private async Task<FakeAppUser> retrieveCurrentUser(IServiceProvider sp)
        {
            var userContext = getUserContext(sp);
            return (FakeAppUser)await userContext.User();
        }

        private FakeUserContext getUserContext(IServiceProvider sp)
            => (FakeUserContext)sp.GetService<IUserContext>();

        private FakeAppApi getApi(IServiceProvider sp) => (FakeAppApi)sp.GetService<IAppApi>();

    }
}
