using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;
using System.Net;
using System.Web;
using XTI_App.Abstractions;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;
using XTI_WebApp.Extensions;
using XTI_WebApp.Fakes;

namespace XTI_WebApp.AspTests;

#pragma warning disable CS0162
internal sealed class XtiAuthenticationTest
{

    private static readonly string baseUrl = "https://webapps.xartogg.com";
    private static readonly string authenticatorUrl = "https://webapps.xartogg.com/Hub/Current/Auth";
    private static readonly string returnKey = "ABC123";

    [Test]
    public async Task ShouldRedirectToLogin()
    {
        var pathBase = "/Hub/Current";
        var input = await setup(pathBase);
        input.Host.GetTestServer().BaseAddress = new Uri(baseUrl);
        var response = await input.GetAsync("/UserAdmin/Index");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Found));
        Assert.That
        (
            response.Headers.Location?.ToString(),
            Is.EqualTo($"{authenticatorUrl}?returnKey={returnKey}")
        );
    }

    [Test]
    public async Task ShouldRedirectToLogin_WhenCalledFromADifferentApp()
    {
        var pathBase = "/Fake/Current";
        var input = await setup(pathBase);
        input.Host.GetTestServer().BaseAddress = new Uri(baseUrl);
        var response = await input.GetAsync("/UserAdmin/Index");
        Assert.That
        (
            response.Headers.Location?.ToString(),
            Is.EqualTo($"{authenticatorUrl}?returnKey={returnKey}")
        );
    }

    public sealed class MainApp
    {
        public MainApp()
        {
            Action = c => c.ChallengeAsync();
        }

        public Func<HttpContext, Task> Action { get; set; }

        public Task Execute(HttpContext c) => Action(c);
    }

    private async Task<TestInput> setup(string pathBase)
    {
        var hostBuilder = new HostBuilder();
        var appKey = FakeInfo.AppKey;
        var host = await hostBuilder
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .UseUrls("https://webapps.xartogg.com/Hub/Current")
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        config.UseXtiConfiguration(context.HostingEnvironment, appKey.Name.DisplayText, appKey.Type.DisplayText, new string[0]);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton<TestAuthOptions>();
                        services.AddSingleton<DefaultWebAppOptions>();
                        services.AddDataProtection();
                        services.ConfigureXtiCookieAndTokenAuthentication(XtiEnvironment.Parse(context.HostingEnvironment.EnvironmentName), context.Configuration);
                        services.AddFakesForXtiWebApp();
                        services.AddScoped<ILoginReturnKey>(_ => new FakeLoginReturnKey(returnKey));
                        services.AddSingleton<MainApp>();
                        services.AddMvc();
                    })
                    .Configure(app =>
                    {
                        app.UsePathBase(pathBase);
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.Run(c =>
                        {
                            c.Request.PathBase = pathBase;
                            var mainApp = c.RequestServices.GetRequiredService<MainApp>();
                            return mainApp.Execute(c);
                        });
                    });
            })
            .StartAsync();
        var webAppOptions = host.Services.GetRequiredService<DefaultWebAppOptions>();
        webAppOptions.XtiAuthentication.AuthenticatorUrl = authenticatorUrl;
        webAppOptions.XtiAuthentication.JwtSecret = "Secret for token";
        return new TestInput(host);
    }

    private sealed class TestInput
    {
        public TestInput(IHost host)
        {
            Host = host;
            Cookies = new CookieContainer();
        }
        public IHost Host { get; }
        public CookieContainer Cookies { get; }

        public async Task<HttpResponseMessage> GetAsync(string relativeUrl)
        {
            var testServer = Host.GetTestServer();
            var absoluteUrl = new Uri(testServer.BaseAddress, relativeUrl);
            var requestBuilder = testServer.CreateRequest(absoluteUrl.ToString());
            requestBuilder.AddHeader(HeaderNames.Authorization, "Test");
            requestBuilder.AddHeader(HeaderNames.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36 OPR/15.0.1147.100");
            AddCookies(requestBuilder, absoluteUrl);
            var response = await requestBuilder.GetAsync();
            UpdateCookies(response, absoluteUrl);
            return response;
        }

        private void AddCookies(RequestBuilder requestBuilder, Uri absoluteUrl)
        {
            var cookieHeader = Cookies.GetCookieHeader(absoluteUrl);
            if (!string.IsNullOrWhiteSpace(cookieHeader))
            {
                requestBuilder.AddHeader(HeaderNames.Cookie, cookieHeader);
            }
        }

        private void UpdateCookies(HttpResponseMessage response, Uri absoluteUrl)
        {
            if (response.Headers.Contains(HeaderNames.SetCookie))
            {
                var cookies = response.Headers.GetValues(HeaderNames.SetCookie);
                foreach (var cookie in cookies)
                {
                    Cookies.SetCookies(absoluteUrl, cookie);
                }
            }
        }
    }
}