using Microsoft.Extensions.DependencyInjection;
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
        var sp = await Setup();
        var userContext = GetUserContext(sp);
        var userFromContext = await userContext.User();
        Assert.That
        (
            userFromContext.UserName,
            Is.EqualTo(new AppUserName("test.user")),
            "Should retrieve user from source"
        );
    }

    [Test]
    public async Task ShouldRefreshUser()
    {
        var sp = await Setup();
        var appContext = sp.GetRequiredService<FakeAppContext>();
        var sourceUserContext = sp.GetRequiredService<FakeUserContext>();
        var user = await sourceUserContext.User();
        var appSetup = sp.GetRequiredService<FakeAppSetup>();
        var cachedUserContext = sp.GetRequiredService<ICachedUserContext>();
        var cachedUser1 = await cachedUserContext.User();
        var modCategory = GetDefaultModCategory(sp);
        sourceUserContext.SetUserRoles
        (
            modCategory,
            ModifierKey.Default,
            AppRoleName.Admin
        );
        cachedUserContext.ClearCache(user.UserName);
        var cachedUser2 = await cachedUserContext.User();
        var userRoles = await cachedUserContext.UserRoles(cachedUser2, appContext.GetDefaultModifier());
        Assert.That
        (
            userRoles.Select(r => r.Name),
            Has.One.EqualTo(AppRoleName.Admin),
            "Should clear cache"
        );
    }

    private static ModifierCategoryModel GetDefaultModCategory(IServiceProvider sp)
    {
        var appContext = sp.GetRequiredService<FakeAppContext>();
        return appContext.GetModCategory(ModifierCategoryName.Default);
    }

    [Test]
    public async Task ShouldRetrieveDifferentUser()
    {
        var sp = await Setup();
        var userContext = GetUserContext(sp);
        await userContext.User();
        var sourceUserContext = (FakeUserContext)sp.GetRequiredService<ISourceUserContext>();
        var anotherUserName = new AppUserName("another.user");
        sourceUserContext.AddUser(anotherUserName);
        sourceUserContext.SetCurrentUser(anotherUserName);
        var differentUser = await userContext.User();
        Assert.That(differentUser.UserName, Is.EqualTo(anotherUserName));
    }

    [Test]
    public async Task ShouldRetrieveUserRolesFromSource()
    {
        var sp = await Setup();
        var appContext = sp.GetRequiredService<FakeAppContext>();
        var userContext = GetUserContext(sp);
        var user = await userContext.User();
        var modCategory = GetDefaultModCategory(sp);
        var userRoles = await userContext.UserRoles(user, appContext.GetModifier(modCategory, ModifierKey.Default));
        Assert.That
        (
            userRoles.Select(ur => ur.Name).ToArray(),
            Is.EquivalentTo(new[] { FakeAppRoles.Instance.Viewer }),
            "Should retrieve user roles from source"
        );
    }

    [Test]
    public async Task ShouldRetrieveUserRolesFromCache()
    {
        var sp = await Setup();
        var cachedUserContext = sp.GetRequiredService<ICachedUserContext>();
        var cachedUser1 = await cachedUserContext.User();
        var appContext = sp.GetRequiredService<FakeAppContext>();
        var modCategory = GetDefaultModCategory(sp);
        var userContext = sp.GetRequiredService<FakeUserContext>();
        await cachedUserContext.UserRoles(cachedUser1, appContext.GetModifier(modCategory, ModifierKey.Default));
        userContext.SetUserRoles
        (
            modCategory,
            ModifierKey.Default,
            AppRoleName.Admin
        );
        var cachedUser2 = await cachedUserContext.User();
        var userRoles2 = await cachedUserContext.UserRoles(cachedUser2, appContext.GetModifier(modCategory, ModifierKey.Default));
        Assert.That
        (
            userRoles2.Select(r => r.Name),
            Is.EquivalentTo(new[] { FakeAppRoles.Instance.Viewer }),
            "Should retrieve user roles from source"
        );
    }

    private async Task<IServiceProvider> Setup()
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
        var appContext = sp.GetRequiredService<FakeAppContext>();
        var userContext = sp.GetRequiredService<FakeUserContext>();
        var userName = new AppUserName("test.user");
        var user = userContext.AddUser(userName);
        var modCategory = GetDefaultModCategory(sp);
        userContext.SetCurrentUser(userName);
        userContext.SetUserRoles
        (
            modCategory,
            ModifierKey.Default,
            FakeAppRoles.Instance.Viewer
        );
        return sp;
    }

    private CachedUserContext GetUserContext(IServiceProvider sp) =>
        (CachedUserContext)sp.GetRequiredService<IUserContext>();

}