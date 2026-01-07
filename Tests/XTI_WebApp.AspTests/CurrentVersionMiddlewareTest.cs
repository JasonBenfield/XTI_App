using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.Net;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_Core.Fakes;
using XTI_TempLog;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;
using XTI_WebApp.Extensions;
using XTI_WebApp.Fakes;

namespace XTI_WebApp.AspTests;

#pragma warning disable CS0162
internal sealed class CurrentVersionMiddlewareTest
{
    [Test]
    public async Task ShouldRedirectCacheBustToV()
    {
        const string envName = "Production";
        var input = await Setup(envName);
        var uri = $"/Fake/Current/Home?cacheBust={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}";
        input.CurrentAction.Configure
        (
            async c =>
            {
                var action = c.RequestServices.GetRequiredService<LogoutAction>();
                await action.Execute(new LogoutRequest(""), CancellationToken.None);
            }
        );
        var response = await input.GetAsync(uri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
        Assert.That
        (
            response.Headers.Location?.ToString() ?? "",
            Is.EqualTo($"https://localhost/Fake/Current/Home?v={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}")
        );
    }

    [Test]
    public async Task ShouldNotChangeOtherQueryParameters()
    {
        const string envName = "Production";
        var input = await Setup(envName);
        var uri = $"/Fake/Current/Home?cacheBust={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}&A=1&B=2&C=3";
        input.CurrentAction.Configure
        (
            async c =>
            {
                var action = c.RequestServices.GetRequiredService<LogoutAction>();
                await action.Execute(new LogoutRequest(""), CancellationToken.None);
            }
        );
        var response = await input.GetAsync(uri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
        Assert.That
        (
            response.Headers.Location?.ToString() ?? "",
            Is.EqualTo($"https://localhost/Fake/Current/Home?v={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}&A=1&B=2&C=3")
        );
    }

    [Test]
    public async Task ShouldRedirectVToCorrectVersion()
    {
        const string envName = "Production";
        var input = await Setup(envName);
        var uri = $"/Fake/Current/Home?v=V1234";
        input.CurrentAction.Configure
        (
            async c =>
            {
                var action = c.RequestServices.GetRequiredService<LogoutAction>();
                await action.Execute(new LogoutRequest(""), CancellationToken.None);
            }
        );
        var response = await input.GetAsync(uri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
        Assert.That
        (
            response.Headers.Location?.ToString() ?? "",
            Is.EqualTo($"https://localhost/Fake/Current/Home?v={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}")
        );
    }

    [Test]
    public async Task ShouldAddV()
    {
        const string envName = "Production";
        var input = await Setup(envName);
        var uri = $"/Fake/Current/Home";
        input.CurrentAction.Configure
        (
            async c =>
            {
                var action = c.RequestServices.GetRequiredService<LogoutAction>();
                await action.Execute(new LogoutRequest(""), CancellationToken.None);
            }
        );
        var response = await input.GetAsync(uri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
        Assert.That
        (
            response.Headers.Location?.ToString() ?? "",
            Is.EqualTo($"https://localhost/Fake/Current/Home?v={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}")
        );
    }

    [Test]
    public async Task ShouldNotRedirect_WhenVIsCorrect()
    {
        const string envName = "Production";
        var input = await Setup(envName);
        var uri = $"/Fake/Current/Home?v={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}";
        input.CurrentAction.Configure
        (
            async c =>
            {
                var action = c.RequestServices.GetRequiredService<LogoutAction>();
                await action.Execute(new LogoutRequest(""), CancellationToken.None);
            }
        );
        var response = await input.GetAsync(uri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private sealed class CurrentAction
    {
        public CurrentAction()
        {
            Action = (c) =>
           {
               c.Response.StatusCode = StatusCodes.Status200OK;
               c.Request.Method = "POST";
               c.Request.ContentType = WebContentTypes.Json;
               return Config(c);
           };
        }
        public TempLog? TempLog { get; set; }
        public Func<HttpContext, Task> Action { get; }
        private Func<HttpContext, Task> Config { get; set; } = (_) => Task.CompletedTask;

        public CurrentAction Configure(Func<HttpContext, Task> config)
        {
            Config = config;
            return this;
        }
    }

    private async Task<TestInput> Setup(string envName = "Test")
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", envName);
        var hostBuilder = new HostBuilder();
        var appKey = FakeInfo.AppKey;
        var host = await hostBuilder
            .ConfigureAppConfiguration
            (
                (hostingContext, config) =>
                {
                    config.UseXtiConfiguration(hostingContext.HostingEnvironment, appKey.Name.DisplayText, appKey.Type.DisplayText, new string[0]);
                }
            )
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton(_ => XtiEnvironment.Parse(envName));
                        services.AddSingleton<CurrentAction>();
                        services.AddSingleton<TestAuthOptions>();
                        services.AddSingleton<XtiAuthenticationOptions>();
                        services
                            .AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>
                            (
                                "Test",
                                options => { }
                            );
                        services.AddAuthorization(options =>
                        {
                            options.DefaultPolicy =
                                new AuthorizationPolicyBuilder("Test")
                                    .RequireAuthenticatedUser()
                                    .Build();
                        });
                        services.AddFakesForXtiWebApp();
                        services.AddScoped<XtiRequestContext>();
                        services.AddSingleton<FakeAppContext>();
                        services.AddSingleton<ISourceAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
                        services.AddSingleton<CachedAppContext>();
                        services.AddSingleton<IAppContext>(sp => sp.GetRequiredService<CachedAppContext>());
                        services.AddSingleton<FakeUserContext>();
                        services.AddSingleton<ISourceUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
                        services.AddSingleton<CachedUserContext>();
                        services.AddSingleton<IUserContext>(sp => sp.GetRequiredService<CachedUserContext>());
                        services.AddSingleton(sp => FakeInfo.AppKey);
                        services.AddSingleton<FakeAppOptions>();
                        services.AddScoped<FakeAppApiFactory>();
                        services.AddScoped<AppApiFactory>(sp => sp.GetRequiredService<FakeAppApiFactory>());
                        services.AddScoped(sp => sp.GetRequiredService<FakeAppApiFactory>().CreateForSuperUser());
                        services.AddSingleton<IAnonClient, FakeAnonClient>();
                        services.AddScoped<FakeAppSetup>();
                        services.AddMvc();
                    })
                    .Configure(app =>
                    {
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.UseXti();
                        app.Run(async (c) =>
                        {
                            var currentAction = c.RequestServices.GetRequiredService<CurrentAction>();
                            currentAction.TempLog = c.RequestServices.GetRequiredService<TempLog>();
                            await currentAction.Action(c);
                        });
                    });
            })
            .StartAsync();
        var authOptions = host.Services.GetRequiredService<XtiAuthenticationOptions>();
        authOptions.JwtSecret = "JwtSecret";
        var setup = host.Services.GetRequiredService<FakeAppSetup>();
        await setup.Run(AppVersionKey.Current, ct: default);
        var userContext = host.Services.GetRequiredService<FakeUserContext>();
        userContext.AddUser(new AppUserName("xartogg"));
        return new TestInput(host);
    }

    private sealed class TestInput
    {
        public TestInput(IHost host)
        {
            Host = host;
            Clock = (FakeClock)host.Services.GetRequiredService<IClock>();
            TestAuthOptions = host.Services.GetRequiredService<TestAuthOptions>();
            Cookies = new CookieContainer();
            CurrentAction = host.Services.GetRequiredService<CurrentAction>();
            AppContext = host.Services.GetRequiredService<FakeAppContext>();
            UserContext = host.Services.GetRequiredService<FakeUserContext>();
        }
        public IHost Host { get; }
        public CookieContainer Cookies { get; }
        public FakeClock Clock { get; }
        public TestAuthOptions TestAuthOptions { get; }
        public CurrentAction CurrentAction { get; }
        public FakeAppContext AppContext { get; }
        public FakeUserContext UserContext { get; }

        public async Task<HttpResponseMessage> GetAsync(string relativeUrl)
        {
            var testServer = Host.GetTestServer();
            testServer.BaseAddress = new Uri("https://localhost");
            var absoluteUrl = new Uri(testServer.BaseAddress, relativeUrl);
            var requestBuilder = testServer.CreateRequest(absoluteUrl.ToString());
            requestBuilder.AddHeader(HeaderNames.Authorization, "Test");
            requestBuilder.AddHeader(HeaderNames.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36 OPR/15.0.1147.100");
            AddCookies(requestBuilder, absoluteUrl);
            var response = await requestBuilder.GetAsync();
            UpdateCookies(response, absoluteUrl);
            return response;
        }

        public async Task<HttpResponseMessage> PostAsync<TModel, TResult>(AppApiAction<TModel, TResult> action, TModel data)
        {
            CurrentAction
                .Configure
                (
                    async (c) =>
                    {
                        var resultData = await action.Execute(data);
                        var serializedResultData = XtiSerializer.Serialize(resultData);
                        var bytes = System.Text.Encoding.UTF8.GetBytes(serializedResultData);
                        await c.Response.BodyWriter.WriteAsync(bytes);
                    }
                );
            var response = await PostAsync(action.Path.Value(), data!);
            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(string relativeUrl, object data)
        {
            var testServer = Host.GetTestServer();
            testServer.BaseAddress = new Uri("https://localhost");
            var absoluteUrl = new Uri(testServer.BaseAddress, relativeUrl);
            var requestBuilder = testServer.CreateRequest(absoluteUrl.ToString());
            requestBuilder.AddHeader(HeaderNames.Authorization, "Test");
            requestBuilder.AddHeader(HeaderNames.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36 OPR/15.0.1147.100");
            AddCookies(requestBuilder, absoluteUrl);
            requestBuilder.And
            (
                r => r.Content = new StringContent(XtiSerializer.Serialize(data), System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json"))
            );
            var response = await requestBuilder.PostAsync();
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