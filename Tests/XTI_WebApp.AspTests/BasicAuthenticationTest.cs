using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.Text;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Extensions;
using XTI_WebApp.Fakes;

namespace XTI_WebApp.AspTests;

internal sealed class FakeBasicAuthValidator : IBasicAuthValidator
{
    private readonly Dictionary<string, string> creds = new()
    {
        { "user1", "Password1" }
    };

    public void ChangeUser1Password(string password)
    {
        creds.Remove("user1");
        creds.Add("user1", password);
    }

    public Task<bool> IsValid(string username, string password)
    {
        var isValid = false;
        if (creds.ContainsKey(username))
        {
            isValid = creds[username] == password;
        }
        return Task.FromResult(isValid);
    }
}

internal sealed class BasicAuthenticationTest
{
    [Test]
    public async Task ShouldAuthenticate()
    {
        var userInfo = new UserInfo(false, "");
        var host = await BuildBasicAuthenticatedHost
        (
            c =>
            {
                userInfo = new UserInfo
                (
                    c.User.Identity?.IsAuthenticated ?? false,
                    c.User.Identity?.Name ?? ""
                );
                return Task.CompletedTask;
            }
        );
        var testServer = host.GetTestServer();
        testServer.BaseAddress = new Uri("https://localhost");
        var absoluteUrl = new Uri(testServer.BaseAddress, "Test");
        var requestBuilder = testServer.CreateRequest(absoluteUrl.ToString());
        var base64Creds = Convert.ToBase64String(Encoding.ASCII.GetBytes($"user1:Password1"));
        var authHeaderValue = $"Basic {base64Creds}";
        requestBuilder.AddHeader(HeaderNames.Authorization, authHeaderValue);
        var response = await requestBuilder.GetAsync();
        Assert.That(userInfo.IsAuthenticated, Is.True, "Should authenticate");
        Assert.That(userInfo.UserName, Is.EqualTo("user1"), "Should authenticate");
    }

    [Test]
    public async Task ShouldNotAuthenticate_WhenPasswordIsIncorrect()
    {
        var userInfo = new UserInfo(false, "");
        var host = await BuildBasicAuthenticatedHost
        (
            c =>
            {
                userInfo = new UserInfo
                (
                    c.User.Identity?.IsAuthenticated ?? false,
                    c.User.Identity?.Name ?? ""
                );
                return Task.CompletedTask;
            }
        );
        var testServer = host.GetTestServer();
        testServer.BaseAddress = new Uri("https://localhost");
        var absoluteUrl = new Uri(testServer.BaseAddress, "Test");
        var requestBuilder = testServer.CreateRequest(absoluteUrl.ToString());
        var base64Creds = Convert.ToBase64String(Encoding.ASCII.GetBytes($"user1:Password2"));
        var authHeaderValue = $"Basic {base64Creds}";
        requestBuilder.AddHeader(HeaderNames.Authorization, authHeaderValue);
        var response = await requestBuilder.GetAsync();
        Assert.That(userInfo.IsAuthenticated, Is.False, "Should not authenticate when password is incorrect");
    }

    [Test]
    public async Task ShouldCacheAuthentication()
    {
        var userInfo = new UserInfo(false, "");
        var host = await BuildBasicAuthenticatedHost
        (
            c =>
            {
                userInfo = new UserInfo
                (
                    c.User.Identity?.IsAuthenticated ?? false,
                    c.User.Identity?.Name ?? ""
                );
                return Task.CompletedTask;
            }
        );
        var testServer = host.GetTestServer();
        testServer.BaseAddress = new Uri("https://localhost");
        var absoluteUrl = new Uri(testServer.BaseAddress, "Test");
        var requestBuilder1 = testServer.CreateRequest(absoluteUrl.ToString());
        var base64Creds = Convert.ToBase64String(Encoding.ASCII.GetBytes($"user1:Password1"));
        var authHeaderValue = $"Basic {base64Creds}";
        requestBuilder1.AddHeader(HeaderNames.Authorization, authHeaderValue);
        await requestBuilder1.GetAsync();
        var validator = host.Services.GetRequiredService<FakeBasicAuthValidator>();
        validator.ChangeUser1Password("PasswordChanged");
        var requestBuilder2 = testServer.CreateRequest(absoluteUrl.ToString());
        requestBuilder2.AddHeader(HeaderNames.Authorization, authHeaderValue);
        await requestBuilder2.GetAsync();
        Assert.That(userInfo.IsAuthenticated, Is.True, "Should authenticate");
        Assert.That(userInfo.UserName, Is.EqualTo("user1"), "Should authenticate");
    }

    private static async Task<IHost> BuildBasicAuthenticatedHost(Func<HttpContext, Task> config)
    {
        var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices
                    (
                        (context, services) =>
                        {
                            const string defaultScheme = "xti_default";
                            services.AddMemoryCache();
                            services.AddHttpContextAccessor();
                            services.AddSingleton<FakeBasicAuthValidator>();
                            services.AddSingleton<IBasicAuthValidator>(sp => sp.GetRequiredService<FakeBasicAuthValidator>());
                            services.AddFakesForXtiWebApp();
                            services
                                .AddAuthentication(defaultScheme)
                                .AddBasicAuthentication()
                                .AddPolicyScheme
                                (
                                    defaultScheme,
                                    defaultScheme,
                                    options =>
                                    {
                                        options.ForwardDefaultSelector = (c) =>
                                        {
                                            return BasicAuthentication.DefaultScheme;
                                        };
                                    }
                                );
                            services.AddMvc();
                        }
                    )
                    .Configure
                    (
                        app =>
                        {
                            app.UseAuthentication();
                            app.UseAuthorization();
                            app.Run((c) => config(c));
                        }
                    );
            })
            .StartAsync();
        return host;
    }

    public sealed record UserInfo(bool IsAuthenticated, string UserName);
}
