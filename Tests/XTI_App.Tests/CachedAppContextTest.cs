using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Extensions;

namespace XTI_App.Tests;

internal sealed class CachedAppContextTest
{
    [Test]
    public async Task ShouldRetrieveAppFromSource()
    {
        var services = await setup();
        var appContext = getAppContext(services);
        var appFromContext = await appContext.App();
        var appSetup = services.GetRequiredService<FakeAppSetup>();
        Assert.That(appFromContext.App.ID, Is.EqualTo(appSetup.App.App.ID), "Should retrieve app from source");
        Assert.That(appFromContext.App.AppKey.Name.DisplayText, Is.EqualTo(appSetup.App.App.AppKey.Name.DisplayText), "Should retrieve app from source");
    }

    [Test]
    public async Task ShouldRetrieveAppFromCache()
    {
        var services = await setup();
        var sourceAppContext = getSourceAppContext(services);
        var cachedAppContext = getAppContext(services);
        var cachedApp1 = await cachedAppContext.App();
        var originalTitle = cachedApp1.App.AppKey.Name.DisplayText;
        var originalApp = await sourceAppContext.App();
        var originalVersionName = originalApp.App.VersionName;
        sourceAppContext.Update
        (
            originalApp,
            a => a with
            {
                App = new AppModel
                (
                    originalApp.App.ID,
                    originalApp.App.AppKey,
                    new AppVersionName("New Version Name"),
                    new ModifierKey(originalApp.App.AppKey.Format())
                )
            }
        );
        var cachedApp2 = await cachedAppContext.App();
        Assert.That(cachedApp2.App.VersionName, Is.EqualTo(originalVersionName), "Should retrieve app from cache");
    }

    [Test]
    public async Task ShouldRetrieveAppRolesFromSource()
    {
        var services = await setup();
        var appContext = getAppContext(services);
        var app = await appContext.App();
        var expectedRoleNames = new[]
        {
            FakeInfo.Roles.Viewer,
            FakeInfo.Roles.Manager
        }
        .Union(AppRoleName.DefaultRoles());
        Assert.That
        (
            app.Roles.Select(ar => ar.Name),
            Is.EquivalentTo(expectedRoleNames),
            "Should retrieve app roles from source"
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
        await sp.Setup();
        return sp;
    }

    private FakeAppContext getSourceAppContext(IServiceProvider services) =>
        (FakeAppContext)services.GetRequiredService<ISourceAppContext>();

    private IAppContext getAppContext(IServiceProvider services) =>
        services.GetRequiredService<IAppContext>();
}