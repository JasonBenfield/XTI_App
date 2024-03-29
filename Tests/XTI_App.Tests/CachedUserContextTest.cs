﻿using Microsoft.Extensions.DependencyInjection;
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
        var userContext = getUserContext(sp);
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
        var sp = await Setup();
        var appContext = sp.GetRequiredService<FakeAppContext>();
        var sourceUserContext = sp.GetRequiredService<FakeUserContext>();
        var user = await sourceUserContext.User();
        var appSetup = sp.GetRequiredService<FakeAppSetup>();
        var cachedUserContext = sp.GetRequiredService<ICachedUserContext>();
        var cachedUser1 = await cachedUserContext.User();
        var modCategory = GetDefaultModCategory(sp);
        sourceUserContext.Update
        (
            user,
            u => (u with
            {
                ModifiedRoles = new[]
                {
                    new UserContextRoleModel
                    (
                        modCategory.ModifierCategory,
                        modCategory.Modifier(ModifierKey.Default),
                        new []
                        {
                            appSetup.App.Role(AppRoleName.Admin)
                        }
                    )
                }
            })
        );
        cachedUserContext.ClearCache(user.User.UserName);
        var cachedUser2 = await cachedUserContext.User();
        Assert.That
        (
            cachedUser2.GetRoles(appContext.GetCurrentApp().DefaultModifier()).Select(r => r.Name),
            Has.One.EqualTo(AppRoleName.Admin),
            "Should clear cache"
        );
    }

    private static AppContextModifierCategoryModel GetDefaultModCategory(IServiceProvider sp)
    {
        var appContext = sp.GetRequiredService<FakeAppContext>();
        return appContext.GetModifierCategory(appContext.GetCurrentApp(), ModifierCategoryName.Default);
    }

    [Test]
    public async Task ShouldRetrieveDifferentUser()
    {
        var sp = await Setup();
        var userContext = getUserContext(sp);
        await userContext.User();
        var sourceUserContext = (FakeUserContext)sp.GetRequiredService<ISourceUserContext>();
        var anotherUserName = new AppUserName("another.user");
        sourceUserContext.AddUser(anotherUserName);
        sourceUserContext.SetCurrentUser(anotherUserName);
        var differentUser = await userContext.User();
        Assert.That(differentUser.User.UserName, Is.EqualTo(anotherUserName));
    }

    [Test]
    public async Task ShouldRetrieveUserRolesFromSource()
    {
        var sp = await Setup();
        var appContext = sp.GetRequiredService<FakeAppContext>();
        var userContext = getUserContext(sp);
        var user = await userContext.User();
        var modCategory = GetDefaultModCategory(sp);
        Assert.That
        (
            user.GetRoles(modCategory.Modifier(ModifierKey.Default)).Select(ur => ur.Name),
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
        var userContext = sp.GetRequiredService<FakeUserContext>();
        var appContext = sp.GetRequiredService<FakeAppContext>();
        var user = await userContext.User();
        var modCategory = GetDefaultModCategory(sp);
        userContext.Update
        (
            user,
            u => u with
            {
                ModifiedRoles = new[]
                {
                    new UserContextRoleModel
                    (
                        modCategory.ModifierCategory,
                        modCategory.Modifier( ModifierKey.Default),
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
            cachedUser2.GetRoles(modCategory.Modifier(ModifierKey.Default)).Select(r => r.Name),
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
        var userContext = sp.GetRequiredService<FakeUserContext>();
        var userName = new AppUserName("test.user");
        var user = userContext.AddUser(userName);
        var modCategoryID = GetDefaultModCategory(sp);
        userContext.Update
        (
            user,
            u => u with
            {
                ModifiedRoles = new[]
                {
                    new UserContextRoleModel
                    (
                        modCategoryID.ModifierCategory,
                        modCategoryID.Modifier(ModifierKey.Default),
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