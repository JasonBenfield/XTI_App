using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_Core.Extensions;

namespace XTI_App.Tests;

internal sealed class AuthorizationTest
{
    [Test]
    public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole()
    {
        var services = await setup();
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var adminRole = appSetup.App.Role(AppRoleName.Admin);
        addRolesToUser(services, adminRole);
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
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var viewerRole = appSetup.App.Role(FakeAppRoles.Instance.Viewer);
        addRolesToUser(services, viewerRole);
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
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var viewerRole = appSetup.App.Role(FakeAppRoles.Instance.Viewer);
        addRolesToUser(services, viewerRole);
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
        var user = retrieveCurrentUser(services);
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var adminRole = appSetup.App.Role(AppRoleName.Admin);
        user.AddRole(adminRole);
        var action = api.Employee.AddEmployee;
        setPath(services, action, "IT");
        var hasAccess = await action.HasAccess();
        Assert.That(hasAccess, Is.True, "Should have access when user belongs to an allowed role for modified action");
    }

    [Test]
    public async Task ShouldNotHaveAccessToModifiedAction_WhenUserDoesNotBelongsToAnAllowedRoleForTheModifier_IfTheyBelongToAnAllowedRoleForTheDefaultModifier()
    {
        var services = await setup();
        var user = retrieveCurrentUser(services);
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var adminRole = appSetup.App.Role(AppRoleName.Admin);
        user.AddRole(adminRole);
        var modCategory = appSetup.App.ModCategory(new ModifierCategoryName("Department"));
        var modifier = modCategory.ModifierOrDefault(new ModifierKey("IT"));
        var viewerRole = appSetup.App.Role(FakeAppRoles.Instance.Viewer);
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
        var user = retrieveCurrentUser(services);
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var modCategory = appSetup.App.ModCategory(new ModifierCategoryName("Department"));
        var modifier = modCategory.ModifierOrDefault(new ModifierKey("IT"));
        var adminRole = appSetup.App.Role(AppRoleName.Admin);
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
        var user = retrieveCurrentUser(services);
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var modCategory = appSetup.App.ModCategory(new ModifierCategoryName("Department"));
        var viewerRole = appSetup.App.Role(FakeAppRoles.Instance.Viewer);
        var modifier = modCategory.ModifierOrDefault(new ModifierKey("IT"));
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
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var adminRole = appSetup.App.Role(AppRoleName.Admin);
        addRolesToUser(services, adminRole);
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
        var user = retrieveCurrentUser(services);
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var adminRole = appSetup.App.Role(AppRoleName.Admin);
        var modCategory = appSetup.App.ModCategory(new ModifierCategoryName("Department"));
        var modifier = modCategory.ModifierOrDefault(new ModifierKey("IT"));
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
        var user = retrieveCurrentUser(services);
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var adminRole = appSetup.App.Role(AppRoleName.Admin);
        user.AddRole(adminRole);
        var denyAccessRole = appSetup.App.Role(AppRoleName.DenyAccess);
        var modCategory = appSetup.App.ModCategory(new ModifierCategoryName("Department"));
        var modifier = modCategory.ModifierOrDefault(new ModifierKey("IT"));
        user.AddRoles(modifier, adminRole, denyAccessRole);
        var api = getApi(services);
        var action = api.Employee.AddEmployee;
        setPath(services, action, "IT");
        var hasAccess = await action.HasAccess();
        Assert.That(hasAccess, Is.False, "User should not have access when user has the deny access role");
    }

    private void addRolesToUser(IServiceProvider services, params FakeAppRole[] roles)
    {
        var user = retrieveCurrentUser(services);
        foreach (var role in roles)
        {
            user.AddRole(role);
        }
    }

    private async Task<IServiceProvider> setup()
    {
        var hostBuilder = new XtiHostBuilder();
        hostBuilder.Services.AddServicesForTests();
        hostBuilder.Services.AddScoped<IAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
        hostBuilder.Services.AddScoped<ICurrentUserName>(sp => sp.GetRequiredService<FakeCurrentUserName>());
        hostBuilder.Services.AddScoped<IUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
        var sp = hostBuilder.Build().Scope();
        await sp.Setup();
        var userContext = (FakeUserContext)sp.GetRequiredService<ISourceUserContext>();
        var userName = new AppUserName("someone");
        userContext.AddUser(userName);
        userContext.SetCurrentUser(userName);
        return sp;
    }

    private void setPath(IServiceProvider sp, IAppApiGroup group)
    {
        var pathAccessor = (FakeXtiPathAccessor)sp.GetRequiredService<IXtiPathAccessor>();
        pathAccessor.SetPath(group.Path);
    }

    private void setPath<TModel, TResult>(IServiceProvider sp, AppApiAction<TModel, TResult> action, string department = "")
    {
        var pathAccessor = (FakeXtiPathAccessor)sp.GetRequiredService<IXtiPathAccessor>();
        pathAccessor.SetPath(action.Path.WithModifier(new ModifierKey(department)));
    }

    private FakeAppUser retrieveCurrentUser(IServiceProvider sp)
    {
        var userContext = getUserContext(sp);
        return userContext.User();
    }

    private FakeUserContext getUserContext(IServiceProvider sp)
        => (FakeUserContext)sp.GetRequiredService<IUserContext>();

    private FakeAppApi getApi(IServiceProvider sp) => (FakeAppApi)sp.GetRequiredService<IAppApi>();

}