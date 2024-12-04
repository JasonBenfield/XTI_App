using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Data;
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
        var sp = await Setup();
        AddRolesToUser(sp, AppRoleName.Admin);
        var api = GetApi(sp);
        var action = api.Product.AddProduct;
        SetModifierKey(sp, action);
        Assert.DoesNotThrowAsync
        (
            () => action.Execute(new AddProductModel { Name = "Something" }),
            "Should have access when user belongs to an allowed role"
        );
    }

    [Test]
    public async Task ShouldNotHaveAccess_WhenUserDoesNoBelongToAnAllowedRole()
    {
        var sp = await Setup();
        AddRolesToUser(sp, FakeAppRoles.Instance.Viewer);
        var api = GetApi(sp);
        var action = api.Employee.AddEmployee;
        SetModifierKey(sp, action);
        Assert.ThrowsAsync<AccessDeniedException>
        (
            () => action.Execute(new AddEmployeeModel { Name = "Someone" }),
            "Should not have access when user does not belong to an allowed role"
        );
    }

    [Test]
    public async Task ShouldNotHaveAccess_WhenUserBelongsToADeniedRole()
    {
        var sp = await Setup();
        var appSetup = sp.GetRequiredService<FakeAppSetup>();
        AddRolesToUser(sp, FakeAppRoles.Instance.Viewer);
        var api = GetApi(sp);
        var action = api.Product.AddProduct;
        SetModifierKey(sp, action, "IT");
        Assert.ThrowsAsync<AccessDeniedException>
        (
            () => action.Execute(new AddProductModel { Name = "Something" }),
            "Should not have access when user belongs to a denied role"
        );
    }

    [Test]
    public async Task ShouldHaveAccessToModifiedAction_WhenUserBelongsToAnAllowedRoleForTheDefaultModifier()
    {
        var sp = await Setup();
        var api = GetApi(sp);
        AddRolesToUser(sp, AppRoleName.Admin);
        var action = api.Employee.AddEmployee;
        SetModifierKey(sp, action, "IT");
        Assert.DoesNotThrowAsync
        (
            () => action.Execute(new AddEmployeeModel { Name = "Someone" }),
            "Should have access when user belongs to an allowed role for modified action"
        );
    }

    [Test]
    public async Task ShouldNotHaveAccessToModifiedAction_WhenUserDoesNotBelongsToAnAllowedRoleForTheModifier_IfTheyBelongToAnAllowedRoleForTheDefaultModifier()
    {
        var sp = await Setup();
        AddRolesToUser(sp, AppRoleName.Admin);
        AddRolesToUser(sp, new ModifierKey("IT"), FakeAppRoles.Instance.Viewer);
        var api = GetApi(sp);
        var action = api.Employee.AddEmployee;
        SetModifierKey(sp, action, "IT");
        Assert.ThrowsAsync<AccessDeniedException>
        (
            () => action.Execute(new AddEmployeeModel { Name = "Someone" }),
            "Should not have access when user does not belong to an allowed role for modified action"
        );
    }

    [Test]
    public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole_AndUserHasAccessToTheModifiedAction()
    {
        var sp = await Setup();
        AddRolesToUser(sp, new ModifierKey("IT"), AppRoleName.Admin);
        var api = GetApi(sp);
        var action = api.Employee.AddEmployee;
        SetModifierKey(sp, action, "IT");
        Assert.DoesNotThrowAsync
        (
            () => action.Execute(new AddEmployeeModel { Name = "Someone" }),
            "Should have access when user belongs to an allowed role for modified action"
        );
    }

    [Test]
    public async Task ShouldNotHaveAccess_WhenUserDoesNotBelongToAnAllowedRole_AndUserHasAccessToTheModifiedAction()
    {
        var sp = await Setup();
        var appSetup = sp.GetRequiredService<FakeAppSetup>();
        AddRolesToUser(sp, new ModifierKey("IT"), FakeAppRoles.Instance.Viewer);
        var api = GetApi(sp);
        var action = api.Employee.AddEmployee;
        SetModifierKey(sp, action, "IT");
        Assert.ThrowsAsync<AccessDeniedException>
        (
            () => action.Execute(new AddEmployeeModel { Name = "Someone" }),
            "Should not have access when user does not belong to an allowed role even if they have access to the modified action"
        );
    }

    [Test]
    public async Task ShouldHaveAccess_WhenUserBelongsToAnAllowedRole_AndTheActionHasTheDefaultModifier()
    {
        var sp = await Setup();
        AddRolesToUser(sp, AppRoleName.Admin);
        var api = GetApi(sp);
        var action = api.Employee.AddEmployee;
        SetModifierKey(sp, action);
        Assert.DoesNotThrowAsync
        (
            () => action.Execute(new AddEmployeeModel { Name = "Someone" }),
            "Should have access when user belongs to an allowed role and the action has the default modifier"
        );
    }

    [Test]
    public async Task ShouldNotHaveAccess_WhenUserBelongsToAnAllowedRole_ButDoesNotHaveAccessToTheModifiedAction()
    {
        var sp = await Setup();
        AddRolesToUser(sp, new ModifierKey("IT"), AppRoleName.Admin);
        var api = GetApi(sp);
        var action = api.Employee.AddEmployee;
        SetModifierKey(sp, action, "HR");
        Assert.ThrowsAsync<AccessDeniedException>
        (
            () => action.Execute(new AddEmployeeModel { Name = "Someone" }),
            "Should not have access when user does not belong to an allowed role for modified action"
        );
    }

    [Test]
    public async Task AnonShouldNotHaveAccess_WhenAnonIsNotAllowed()
    {
        var sp = await Setup();
        var userContext = GetUserContext(sp);
        userContext.SetCurrentUser(AppUserName.Anon);
        var api = GetApi(sp);
        var action = api.Home.DoSomething;
        SetModifierKey(sp, action);
        Assert.ThrowsAsync<AccessDeniedException>
        (
            () => action.Execute(new SampleRequestData()),
            "Anon should not have access unless anons are allowed"
        );
    }

    [Test]
    public async Task AnonShouldHaveAccess_WhenAnonIsAllowed()
    {
        var sp = await Setup();
        var userContext = GetUserContext(sp);
        userContext.SetCurrentUser(AppUserName.Anon);
        var api = GetApi(sp);
        var action = api.Login.Authenticate;
        SetModifierKey(sp, action);
        Assert.DoesNotThrowAsync
        (
            () => action.Execute(new EmptyRequest()),
            "Anon should have access when anons are allowed"
        );
    }

    [Test]
    public async Task ShouldNotAllowAccess_WhenTheUserHasTheDenyAccessRole()
    {
        var sp = await Setup();
        AddRolesToUser(sp, AppRoleName.Admin);
        AddRolesToUser(sp, new ModifierKey("IT"), AppRoleName.DenyAccess);
        var api = GetApi(sp);
        var action = api.Employee.AddEmployee;
        SetModifierKey(sp, action, "IT");
        Assert.ThrowsAsync<AccessDeniedException>
        (
            () => action.Execute(new AddEmployeeModel { Name = "Someone" }),
            "User should not have access when user has the deny access role"
        );
    }

    [Test]
    public async Task ShouldNotAllowAccess_WhenUserHasBeenDeactivated()
    {
        var sp = await Setup();
        AddRolesToUser(sp, AppRoleName.Admin);
        var userContext = sp.GetRequiredService<FakeUserContext>();
        userContext.DeactivateUser();
        var api = GetApi(sp);
        var action = api.Employee.AddEmployee;
        SetModifierKey(sp, action, "IT");
        Assert.ThrowsAsync<AccessDeniedException>
        (
            () => action.Execute(new AddEmployeeModel { Name = "Someone" }),
            "User should not have access when user has been deactivated"
        );
    }

    private void AddRolesToUser(IServiceProvider services, params AppRoleName[] roles) =>
        AddRolesToUser(services, ModifierKey.Default, roles);

    private void AddRolesToUser(IServiceProvider services, ModifierKey modifierKey, params AppRoleName[] roles)
    {
        var userContext = services.GetRequiredService<FakeUserContext>();
        userContext.AddRolesToUser
        (
            modifierKey.Equals(ModifierKey.Default) ? ModifierCategoryName.Default : FakeInfo.ModCategories.Department,
            modifierKey,
            roles
        );
    }

    private async Task<IServiceProvider> Setup()
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

    private void SetModifierKey<TModel, TResult>(IServiceProvider sp, AppApiAction<TModel, TResult> action, string department = "")
    {
        var modifierKeyAccessor = sp.GetRequiredService<FakeModifierKeyAccessor>();
        modifierKeyAccessor.SetValue(new ModifierKey(department));
    }

    private FakeUserContext GetUserContext(IServiceProvider sp) => 
        sp.GetRequiredService<FakeUserContext>();

    private FakeAppApi GetApi(IServiceProvider sp) => (FakeAppApi)sp.GetRequiredService<IAppApi>();

}