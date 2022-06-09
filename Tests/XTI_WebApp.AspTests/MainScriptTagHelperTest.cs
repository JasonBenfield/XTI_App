using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XTI_App.Abstractions;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;
using XTI_WebApp.Fakes;
using XTI_WebApp.TagHelpers;

namespace XTI_WebApp.AspTests;

internal sealed class MainScriptTagHelperTest
{
    [Test]
    public async Task ShouldOutputScript()
    {
        var sp = setup();
        var result = await execute(sp);
        Assert.That(result.TagName, Is.EqualTo("script"));
    }

    [Test]
    public async Task ShouldAddSrcAttribute()
    {
        var sp = setup();
        var result = await execute(sp);
        Assert.That(result.Attributes.Count, Is.EqualTo(1));
        Assert.That(result.Attributes[0].Name, Is.EqualTo("src"));
    }

    [Test]
    public async Task ShouldUsePageNameInSrc()
    {
        var sp = setup();
        var tagHelper = sp.GetRequiredService<MainScriptTagHelper>();
        tagHelper.PageName = "home";
        var result = await execute(sp);
        var src = result.Attributes[0].Value;
        Assert.That(src, Does.Contain($"/home.js?"));
    }

    [Test]
    public async Task ShouldIncludeCacheBustFromWebAppOptions()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Production");
        var sp = setup(cacheBust: "X");
        var result = await execute(sp);
        var src = result.Attributes[0].Value;
        Assert.That(src, Does.EndWith("?cacheBust=X"));
    }

    [Test]
    public async Task ShouldIncludeImplicitCacheBust_WhenNotSetInOptions()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Production");
        var sp = setup("/Shared/Current/Home/Index");
        var result = await execute(sp);
        var src = result.Attributes[0].Value;
        var cacheBust = await sp.GetRequiredService<CacheBust>().Value();
        Console.WriteLine($"cacheBust: {cacheBust}");
        Assert.That(src, Does.EndWith($"?cacheBust={cacheBust}"));
    }

    [Test]
    [TestCase("Development", "dev")]
    [TestCase("Test", "dev")]
    [TestCase("Staging", "dist")]
    [TestCase("Production", "dist")]
    public async Task ShouldChangePathBasedOnEnvironment(string envName, string expectedPath)
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", envName);
        var sp = setup();
        var result = await execute(sp);
        var src = result.Attributes[0].Value;
        Assert.That(src, Does.StartWith($"/js/{expectedPath}/"));
    }

    private IServiceProvider setup(string path = "/Shared/Current/Home/Index", string cacheBust = "")
    {
        var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Test";
        var hostBuilder = new XtiHostBuilder();
        hostBuilder.Configuration.AddInMemoryCollection(new[]
        {
            KeyValuePair.Create("WebApp:CacheBust", cacheBust)
        });
        hostBuilder.Services.AddFakesForXtiWebApp();
        hostBuilder.Services.AddSingleton(sp => FakeInfo.AppKey);
        hostBuilder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
        hostBuilder.Services.AddMemoryCache();
        hostBuilder.Services.AddScoped<AppClientDomainSelector>();
        hostBuilder.Services.AddScoped<AppClients>();
        hostBuilder.Services.AddScoped<CacheBust>();
        hostBuilder.Services.AddScoped<MainScriptTagHelper>();
        hostBuilder.Services.AddSingleton(_ => XtiEnvironment.Parse(envName));
        var sp = hostBuilder.Build().Scope();
        var xtiPath = XtiPath.Parse(path);
        var pathAccessor = (FakeXtiPathAccessor)sp.GetRequiredService<IXtiPathAccessor>();
        pathAccessor.SetPath(XtiPath.Parse(path));
        var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            RequestServices = sp
        };
        httpContextAccessor.HttpContext.Request.Path = path;
        var appContext = sp.GetRequiredService<FakeAppContext>();
        var app = appContext.AddApp(FakeInfo.AppKey);
        appContext.SetCurrentApp(app);
        var tagHelper = sp.GetRequiredService<MainScriptTagHelper>();
        tagHelper.PageName = "home";
        tagHelper.ViewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext()
        {
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor
            {
                RouteValues = new Dictionary<string, string?>()
            },
            HttpContext = httpContextAccessor.HttpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData()
        };
        return sp;
    }

    private async Task<TagHelperOutput> execute(IServiceProvider sp)
    {
        var tagHelperContext = new TagHelperContext
        (
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N")
        );
        var tagHelperOutput = new TagHelperOutput
        (
            "xti-main-script",
            new TagHelperAttributeList(),
            (result, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            }
        );
        var tagHelper = sp.GetRequiredService<MainScriptTagHelper>();
        await tagHelper.ProcessAsync(tagHelperContext, tagHelperOutput);
        return tagHelperOutput;
    }
}