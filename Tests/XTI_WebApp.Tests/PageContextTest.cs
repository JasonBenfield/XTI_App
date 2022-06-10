using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Fakes;

namespace XTI_WebApp.Tests;

internal sealed class PageContextTest
{
    [Test]
    public async Task ShouldSetAppTitle()
    {
        var services = await setup(AppVersionKey.Current);
        var pageContext = await execute(services);
        var appContext = services.GetRequiredService<IAppContext>();
        var app = await appContext.App();
        Assert.That(pageContext.AppTitle, Is.EqualTo(app.Title), "Should set app title");
    }

    [Test]
    public async Task ShouldSetWebAppDomains()
    {
        var domain = "webapps.xartogg.com";
        var services = await setup(AppVersionKey.Current, domain);
        var pageContext = await execute(services);
        Assert.That
        (
            pageContext.WebAppDomains.Select(d => new AppVersionDomain(d.App, d.Version, d.Domain)),
            Is.EqualTo(new[] { new AppVersionDomain("Fake", "Current", domain) }),
            "Should set web app domains"
        );
    }

    [Test]
    public async Task ShouldSetEnvironmentName()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Staging");
        var input = await setup(AppVersionKey.Current);
        var pageContext = await execute(input);
        Assert.That(pageContext.EnvironmentName, Is.EqualTo("Staging"), "Should set environment name");
    }

    [Test]
    public async Task ShouldSetUserName()
    {
        var services = await setup(AppVersionKey.Current);
        var userName = new AppUserName("someone");
        var userContext = services.GetRequiredService<FakeUserContext>();
        userContext.SetCurrentUser(userName);
        var pageContext = await execute(services);
        Assert.That(pageContext.IsAuthenticated, Is.True, "Should be authenticated");
        Assert.That(pageContext.UserName, Is.EqualTo(userName.Value), "Should set user name");
    }

    [Test]
    public async Task ShouldSetUserNameToBlankForAnon()
    {
        var services = await setup(AppVersionKey.Current);
        var userContext = services.GetRequiredService<FakeUserContext>();
        userContext.SetCurrentUser(AppUserName.Anon);
        var pageContext = await execute(services);
        Assert.That(pageContext.IsAuthenticated, Is.False, "Should not be authenticated");
        Assert.That(pageContext.UserName, Is.EqualTo(""), "Should set user name to blank for anon");
    }

    [Test]
    public async Task ShouldSetCacheBustToCurrentVersion()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Production");
        var services = await setup(AppVersionKey.Current);
        var pageContext = await execute(services);
        var appContext = services.GetRequiredService<FakeAppContext>();
        var version = await appContext.Version();
        Assert.That(pageContext?.CacheBust, Is.EqualTo(version.Key().DisplayText), "Should set cacheBust to current version");
    }

    [Test]
    public async Task ShouldNotSetCacheBust_WhenVersionIsNotCurrent()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Production");
        var services = await setup(new AppVersionKey(2));
        var pageContext = await execute(services);
        Assert.That(pageContext.CacheBust, Is.EqualTo(""), "Should not set cacheBust when version is null");
    }

    private static async Task<PageContextRecord> execute(IServiceProvider services)
    {
        var pageContext = services.GetRequiredService<IPageContext>();
        var serialized = await pageContext.Serialize();
        var deserialized = XtiSerializer.Deserialize<PageContextRecord>(serialized);
        return deserialized;
    }

    private class PageContextRecord
    {
        public AppVersionDomainRecord[] WebAppDomains { get; set; } = new AppVersionDomainRecord[0];
        public string CacheBust { get; set; } = "";
        public string AppTitle { get; set; } = "";
        public string PageTitle { get; set; } = "";
        public string UserName { get; set; } = "";
        public bool IsAuthenticated { get; set; }
        public string EnvironmentName { get; set; } = "";
    }

    private sealed class AppVersionDomainRecord
    {
        public string App { get; set; } = "";
        public string Version { get; set; } = "";
        public string Domain { get; set; } = "";
    }

    private async Task<IServiceProvider> setup(AppVersionKey versionKey, string domain = "www.xartogg.com")
    {
        var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Test";
        var hostBuilder = new XtiHostBuilder();
        hostBuilder.Services.AddMemoryCache();
        hostBuilder.Services.AddSingleton(_ => XtiEnvironment.Parse(envName));
        hostBuilder.Services.AddFakesForXtiWebApp();
        hostBuilder.Services.AddSingleton<FakeAppOptions>();
        hostBuilder.Services.AddSingleton(sp => FakeInfo.AppKey);
        hostBuilder.Services.AddScoped<FakeAppApiFactory>();
        hostBuilder.Services.AddScoped<AppApiFactory>(sp => sp.GetRequiredService<FakeAppApiFactory>());
        hostBuilder.Services.AddScoped<FakeAppSetup>();
        hostBuilder.Services.AddSingleton(_ => new FakeAppClientDomain(domain));
        hostBuilder.Services.AddSingleton(sp =>
        {
            var cache = sp.GetRequiredService<IMemoryCache>();
            var domains = new AppClientDomainSelector();
            domains.AddAppClientDomain(() => sp.GetRequiredService<FakeAppClientDomain>());
            var appClients = new AppClients(cache, domains);
            appClients.AddAppVersion("Fake", "Current");
            return appClients;
        });
        var sp = hostBuilder.Build().Scope();
        var pathAccessor = (FakeXtiPathAccessor)sp.GetRequiredService<IXtiPathAccessor>();
        pathAccessor.SetPath
        (
            new XtiPath
            (
                FakeInfo.AppKey,
                versionKey,
                new ResourceGroupName(""),
                new ResourceName(""),
                ModifierKey.Default
            )
        );
        var setup = sp.GetRequiredService<FakeAppSetup>();
        await setup.Run(AppVersionKey.Current);
        var userContext = sp.GetRequiredService<FakeUserContext>();
        userContext.AddUser(new AppUserName("someone"));
        return sp;
    }

    private sealed class FakeAppClientDomain : IAppClientDomain
    {
        private readonly string domain;

        public FakeAppClientDomain(string domain)
        {
            this.domain = domain;
        }

        public Task<string> Value(string appName, string version) => Task.FromResult(domain);
    }
}