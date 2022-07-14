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
        Assert.That
        (
            userFromContext.User.UserName,
            Is.EqualTo(new AppUserName("test.user")),
            "Should retrieve user from source"
        );
    }

    [Test]
    public async Task ShouldRefreshUser()
    {
        var services = await setup();
        var sourceUserContext = services.GetRequiredService<FakeUserContext>();
        var user = await sourceUserContext.User();
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        var cachedUserContext = services.GetRequiredService<ICachedUserContext>();
        var cachedUser1 = await cachedUserContext.User();
        sourceUserContext.Update
        (
            user,
            u => u with
            {
                ModifiedRoles = new[]
                {
                    new UserContextRoleModel
                    (
                        ModifierKey.Default,
                        new []
                        {
                            appSetup.App.Role(AppRoleName.Admin)
                        }
                    )
                }
            }
        );
        cachedUserContext.ClearCache(user.User.UserName);
        var cachedUser2 = await cachedUserContext.User();
        Assert.That
        (
            cachedUser2.GetRoles(ModifierKey.Default).Select(r => r.Name),
            Has.One.EqualTo(AppRoleName.Admin),
            "Should clear cache"
        );
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
        Assert.That(differentUser.User.UserName, Is.EqualTo(anotherUserName));
    }

    [Test]
    public async Task ShouldRetrieveUserRolesFromSource()
    {
        var services = await setup();
        var userContext = getUserContext(services);
        var user = await userContext.User();
        Assert.That
        (
            user.GetRoles(ModifierKey.Default).Select(ur => ur.Name),
            Is.EquivalentTo(new[] { FakeAppRoles.Instance.Viewer }),
            "Should retrieve user roles from source"
        );
    }

    [Test]
    public async Task ShouldRetrieveUserRolesFromCache()
    {
        var services = await setup();
        var cachedUserContext = services.GetRequiredService<ICachedUserContext>();
        var cachedUser1 = await cachedUserContext.User();
        var userContext = services.GetRequiredService<FakeUserContext>();
        var appContext = services.GetRequiredService<FakeAppContext>();
        var user = await userContext.User();
        userContext.Update
        (
            user,
            u => u with
            {
                ModifiedRoles = new[]
                {
                    new UserContextRoleModel
                    (
                        ModifierKey.Default,
                        new []
                        {
                            appContext.GetCurrentApp().Role(AppRoleName.Admin)
                        }
                    )
                }
            }
        );
        var cachedUser2 = await cachedUserContext.User();
        Assert.That
        (
            cachedUser2.GetRoles(ModifierKey.Default).Select(r => r.Name),
            Is.EquivalentTo(new[] { FakeAppRoles.Instance.Viewer }),
            "Should retrieve user roles from source"
        );
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
        var userContext = sp.GetRequiredService<FakeUserContext>();
        var userName = new AppUserName("test.user");
        var user = userContext.AddUser(userName);
        userContext.Update
        (
            user,
            u => u with
            {
                ModifiedRoles = new[]
                {
                    new UserContextRoleModel
                    (
                        ModifierKey.Default,
                        new []
                        {
                            fakeSetup.App.Role(FakeAppRoles.Instance.Viewer)
                        }
                    )
                }
            }
        );
        userContext.SetCurrentUser(userName);
        return sp;
    }

    private CachedUserContext getUserContext(IServiceProvider sp)
        => (CachedUserContext)sp.GetRequiredService<IUserContext>();

}