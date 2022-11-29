using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;
using XTI_WebApp.Extensions;
using XTI_WebApp.Fakes;

namespace XTI_WebApp.AspTests;

internal sealed class GetMenuLinksTest
{
    [Test]
    public async Task ShouldGetMenuLinks()
    {
        var sp = await Setup();
        var menuName = "Menu";
        var links = new[] { new LinkModel("link1", "Link 1", "/Link1") };
        var linkService = sp.GetRequiredService<FakeMenuDefinitionBuilder>();
        linkService.AddMenu(menuName, links);
        var actualLinks = await GetLinks(sp, menuName);
        Assert.That(actualLinks, Is.EqualTo(links), "Should get links");
    }

    [Test]
    public async Task ShouldGetMenuLinksForAnotherMenu()
    {
        var sp = await Setup();
        var menuName = "Menu1";
        var linkService = sp.GetRequiredService<FakeMenuDefinitionBuilder>();
        linkService.AddMenu(menuName, new LinkModel("link1", "Link 1", "/Link1"));
        var otherMenuName = "Menu2";
        var otherLinks = new[] { new LinkModel("link2", "Link 2", "/Link2") };
        linkService.AddMenu(otherMenuName, otherLinks);
        var actualLinks = await GetLinks(sp, otherMenuName);
        Assert.That(actualLinks, Is.EqualTo(otherLinks), "Should get links for another menu");
    }

    [Test]
    public async Task ShouldNotIncludeLink_WhenUserDoesNotHaveAccess()
    {
        var sp = await Setup();
        var menuName = "Menu";
        var linkService = sp.GetRequiredService<FakeMenuDefinitionBuilder>();
        linkService.AddMenu
        (
            menuName,
            new LinkModel("link1", "Link 1", "/Link1"),
            new LinkModel("link2", "Link 2", $"~/Employee/AddEmployee")
        );
        var userContext = sp.GetRequiredService<FakeUserContext>();
        userContext.SetCurrentUser(AppUserName.Anon);
        var actualLinks = await GetLinks(sp, menuName);
        Assert.That(actualLinks.Select(l => l.LinkName), Has.None.EqualTo("link2"));
    }

    [Test]
    public async Task ShouldIncludeLink_WhenUserHasAccess()
    {
        var sp = await Setup();
        var menuName = "Menu";
        var linkService = sp.GetRequiredService<FakeMenuDefinitionBuilder>();
        linkService.AddMenu
        (
            menuName,
            new LinkModel("link1", "Link 1", "/Link1"),
            new LinkModel("link2", "Link 2", $"~/Employee/AddEmployee")
        );
        var userContext = sp.GetRequiredService<FakeUserContext>();
        userContext.AddRolesToUser(AppRoleName.Admin);
        var actualLinks = await GetLinks(sp, menuName);
        Assert.That(actualLinks.Select(l => l.LinkName), Has.One.EqualTo("link2"));
    }

    [Test]
    public async Task ShouldExpandUrl()
    {
        var sp = await Setup();
        var menuName = "Menu";
        var linkService = sp.GetRequiredService<FakeMenuDefinitionBuilder>();
        linkService.AddMenu
        (
            menuName,
            new LinkModel("link1", "Link 1", "/Link1"),
            new LinkModel("link2", "Link 2", $"~/Employee/AddEmployee")
        );
        var userContext = sp.GetRequiredService<FakeUserContext>();
        userContext.AddRolesToUser(AppRoleName.Admin);
        var actualLinks = await GetLinks(sp, menuName);
        var link2 = actualLinks.Where(l => l.LinkName == "link2").First();
        Assert.That(link2.Url, Is.EqualTo("/Fake/Current/Employee/AddEmployee"));
    }

    [Test]
    public async Task ShouldReplaceFullName()
    {
        var sp = await Setup();
        var menuName = "Menu";
        var linkService = sp.GetRequiredService<FakeMenuDefinitionBuilder>();
        linkService.AddMenu
        (
            menuName,
            new LinkModel("link1", "Link 1", "/Link1"),
            new LinkModel("link2", "Link 2", $"~/Employee/AddEmployee"),
            new LinkModel("link3", "{User.FullName}", "")
        );
        var userContext = sp.GetRequiredService<FakeUserContext>();
        userContext.AddRolesToUser(AppRoleName.Admin);
        var actualLinks = await GetLinks(sp, menuName);
        var link3 = actualLinks.Where(l => l.LinkName == "link3").First();
        Assert.That(link3.DisplayText, Is.EqualTo("Someone"));
    }

    private async Task<IServiceProvider> Setup()
    {
        var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Test";
        var xtiEnv = XtiEnvironment.Parse(envName);
        var hostBuilder = new XtiHostBuilder();
        hostBuilder.Services.AddMemoryCache();
        hostBuilder.Services.AddSingleton(_ => xtiEnv);
        hostBuilder.Services.AddFakesForXtiWebApp();
        hostBuilder.Services.AddSingleton(sp => FakeInfo.AppKey);
        hostBuilder.Services.AddSingleton(sp => AppVersionKey.Current);
        hostBuilder.Services.AddScoped<ITransformedLinkFactory, DefaultTransformedLinkFactory>();
        hostBuilder.Services.AddScoped<GetMenuLinksAction>();
        hostBuilder.Services.AddSingleton<FakeAppOptions>();
        hostBuilder.Services.AddScoped<FakeAppApiFactory>();
        hostBuilder.Services.AddScoped<AppApiFactory>(sp => sp.GetRequiredService<FakeAppApiFactory>());
        hostBuilder.Services.AddScoped(sp => sp.GetRequiredService<FakeAppApiFactory>().CreateForSuperUser());
        hostBuilder.Services.AddScoped<FakeAppSetup>();
        hostBuilder.Services.AddScoped<IAppSetup>(sp => sp.GetRequiredService<FakeAppSetup>());
        hostBuilder.Services.AddHttpContextAccessor();
        hostBuilder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        hostBuilder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
        hostBuilder.Services.AddScoped<IAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
        hostBuilder.Services.AddScoped<ICurrentUserName>(sp => sp.GetRequiredService<FakeCurrentUserName>());
        hostBuilder.Services.AddScoped<IUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
        hostBuilder.Services.AddSingleton<FakeMenuDefinitionBuilder>();
        hostBuilder.Services.AddSingleton<IMenuDefinitionBuilder>(sp => sp.GetRequiredService<FakeMenuDefinitionBuilder>());
        hostBuilder.Services.AddSingleton(sp => sp.GetRequiredService<IMenuDefinitionBuilder>().Build());
        var sp = hostBuilder.Build().Scope();
        var pathAccessor = (FakeXtiPathAccessor)sp.GetRequiredService<IXtiPathAccessor>();
        pathAccessor.SetPath
        (
            new XtiPath
            (
                FakeInfo.AppKey,
                AppVersionKey.Current,
                new ResourceGroupName(""),
                new ResourceName(""),
                ModifierKey.Default
            )
        );
        var setup = sp.GetRequiredService<FakeAppSetup>();
        await setup.Run(AppVersionKey.Current);
        var userContext = sp.GetRequiredService<FakeUserContext>();
        var userName = new AppUserName("Someone");
        userContext.AddUser(userName);
        userContext.SetCurrentUser(userName);
        return sp;
    }

    public Task<LinkModel[]> GetLinks(IServiceProvider sp, string menuName)
    {
        var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.Request.PathBase = "/Fake/Current";
        httpContextAccessor.HttpContext.Request.Path = "/User/GetMenuLinks";
        var action = sp.GetRequiredService<GetMenuLinksAction>();
        return action.Execute(menuName, default);
    }

    private sealed class FakeMenuDefinitionBuilder : IMenuDefinitionBuilder
    {
        private readonly List<MenuDefinition> menuDefinitions = new();

        public void AddMenu(string menuName, params LinkModel[] links)
        {
            menuDefinitions.Add(new MenuDefinition(menuName, links));
        }

        public AppMenuDefinitions Build() => new AppMenuDefinitions(menuDefinitions.ToArray());
    }
}
