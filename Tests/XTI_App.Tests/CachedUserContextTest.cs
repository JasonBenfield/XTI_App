using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Extensions;

namespace XTI_App.Tests;

internal sealed class CachedUserContextTest
{
    [Test]
    public async Task ShouldRetrieveUserFromSource()
    {
        var services = await setup();
        var userContext = getUserContext(services);
        var userFromContext = await userContext.User();
        var sourceUser = testUser(services);
        Assert.That(userFromContext.ID, Is.EqualTo(sourceUser.ID), "Should retrieve user from source");
        Assert.That(userFromContext.UserName(), Is.EqualTo(sourceUser.UserName()), "Should retrieve user from source");
    }

    [Test]
    public async Task ShouldRefreshUser()
    {
        var services = await setup();
        var userContext = getUserContext(services);
        var user = await userContext.User();
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var modCategory = appSetup.App.ModCategory(ModifierCategoryName.Default);
        var modifier = modCategory.ModifierOrDefault(ModifierKey.Default);
        await user.Roles(modifier);
        var adminRole = appSetup.App.Role(AppRoleName.Admin);
        var sourceUser = testUser(services);
        sourceUser.AddRoles(modifier, adminRole);
        userContext.ClearCache(user.UserName());
        var userRoles = await user.Roles(modifier);
        Assert.That(userRoles.Select(r => r.Name()), Has.One.EqualTo(AppRoleName.Admin), "Should clear cache");
    }

    [Test]
    public async Task ShouldRetrieveDifferentUser()
    {
        var services = await setup();
        var userContext = getUserContext(services);
        await userContext.User();
        var sourceUserContext = (FakeUserContext)services.GetRequiredService<ISourceUserContext>();
        var anotherUserName = new AppUserName("another.user");
        sourceUserContext.AddUser(anotherUserName);
        sourceUserContext.SetCurrentUser(anotherUserName);
        var differentUser = await userContext.User();
        Assert.That(differentUser.UserName(), Is.EqualTo(anotherUserName));
    }

    [Test]
    public async Task ShouldRetrieveUserRolesFromSource()
    {
        var services = await setup();
        var userContext = getUserContext(services);
        var user = await userContext.User();
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var modCategory = appSetup.App.ModCategory(ModifierCategoryName.Default);
        var modifier = modCategory.ModifierOrDefault(ModifierKey.Default);
        var userRoles = await user.Roles(modifier);
        var viewerRole = appSetup.App.Role(FakeAppRoles.Instance.Viewer);
        Assert.That(userRoles.Select(ur => ur.ID), Is.EquivalentTo(new[] { viewerRole.ID }), "Should retrieve user roles from source");
    }

    [Test]
    public async Task ShouldRetrieveUserRolesFromCache()
    {
        var services = await setup();
        var userContext = getUserContext(services);
        var user = await userContext.User();
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var modCategory = appSetup.App.ModCategory(ModifierCategoryName.Default);
        var modifier = modCategory.ModifierOrDefault(ModifierKey.Default);
        var userRoles = await user.Roles(modifier);
        var adminRole = appSetup.App.Role(AppRoleName.Admin);
        var sourceUser = testUser(services);
        sourceUser.AddRoles(modifier, adminRole);
        var cachedUser = await userContext.User();
        var cachedUserRoles = await cachedUser.Roles(modifier);
        var viewerRole = appSetup.App.Role(FakeAppRoles.Instance.Viewer);
        Assert.That(userRoles.Select(ur => ur.ID), Is.EquivalentTo(new[] { viewerRole.ID }), "Should retrieve user roles from source");
    }

    private async Task<IServiceProvider> setup()
    {
        var hostBuilder = new XtiHostBuilder(XtiEnvironment.Test);
        hostBuilder.Services.AddServicesForTests();
        hostBuilder.Services.AddScoped<IAppContext>(sp => sp.GetRequiredService<CachedAppContext>());
        hostBuilder.Services.AddScoped<IUserContext>(sp => sp.GetRequiredService<CachedUserContext>());
        var sp = hostBuilder.Build().Scope();
        var xtiPathAccessor = (FakeXtiPathAccessor)sp.GetRequiredService<IXtiPathAccessor>();
        xtiPathAccessor.SetPath(XtiPath.Parse("/Fake/Current/Employees/Index"));
        var fakeSetup = sp.GetRequiredService<FakeAppSetup>();
        await fakeSetup.Run(AppVersionKey.Current);
        var modCategory = fakeSetup.App.ModCategory(ModifierCategoryName.Default);
        var modifier = modCategory.ModifierOrDefault(ModifierKey.Default);
        var userContext = sp.GetRequiredService<FakeUserContext>();
        var userName = new AppUserName("test.user");
        var user = userContext.AddUser(userName);
        var viewerRole = fakeSetup.App.Role(FakeAppRoles.Instance.Viewer);
        user.AddRoles(modifier, viewerRole);
        userContext.SetCurrentUser(userName);
        return sp;
    }

    private CachedUserContext getUserContext(IServiceProvider sp)
        => (CachedUserContext)sp.GetRequiredService<IUserContext>();

    private FakeAppUser testUser(IServiceProvider sp)
    {
        var userContext = sp.GetRequiredService<FakeUserContext>();
        return userContext.User();
    }
}