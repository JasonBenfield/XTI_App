using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_WebApp.Extensions;
using XTI_WebApp.Fakes;

namespace XTI_WebApp.AspTests;

internal sealed class ApiKeyAuthenticationTest
{
    [Test]
    public async Task ShouldAuthenticateByApiKey()
    {
        var userInfo = new UserInfo(false, "");
        var host = await BuildApiKeyAuthenticatedHost
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
        var apiKeyAuthOptions = host.Services.GetRequiredService<ApiKeyAuthenticationOptions>();
        apiKeyAuthOptions.Headers = new[]
        {
            new ApiKeyHeaderOptions
            {
                HeaderName = "XTI_API_KEY",
                ApiKeys = new []
                {
                    new ApiKeyOptions
                    {
                        ApiKey = "ApiKey1",
                        UserName = "User1"
                    }
                }
            }
        };
        var testServer = host.GetTestServer();
        testServer.BaseAddress = new Uri("https://localhost");
        var absoluteUrl = new Uri(testServer.BaseAddress, "Test");
        var requestBuilder = testServer.CreateRequest(absoluteUrl.ToString());
        requestBuilder.AddHeader("XTI_API_KEY", "ApiKey1");
        var response = await requestBuilder.GetAsync();
        Assert.That(userInfo.IsAuthenticated, Is.True, "Should authenticate by API key");
        Assert.That(userInfo.UserName, Is.EqualTo("user1"), "Should authenticate by API key");
    }

    [Test]
    public async Task ShouldNotAuthenticate_WhenApiKeyIsNotFound()
    {
        var userInfo = new UserInfo(false, "");
        var host = await BuildApiKeyAuthenticatedHost
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
        var apiKeyAuthOptions = host.Services.GetRequiredService<ApiKeyAuthenticationOptions>();
        apiKeyAuthOptions.Headers = new[]
        {
            new ApiKeyHeaderOptions
            {
                HeaderName = "XTI_API_KEY",
                ApiKeys = new []
                {
                    new ApiKeyOptions
                    {
                        ApiKey = "ApiKey1",
                        UserName = "User1"
                    }
                }
            }
        };
        var testServer = host.GetTestServer();
        testServer.BaseAddress = new Uri("https://localhost");
        var absoluteUrl = new Uri(testServer.BaseAddress, "Test");
        var requestBuilder = testServer.CreateRequest(absoluteUrl.ToString());
        requestBuilder.AddHeader("XTI_API_KEY", "ApiKey2");
        var response = await requestBuilder.GetAsync();
        Assert.That(userInfo.IsAuthenticated, Is.False, "Should not authenticate when API key is not found");
    }

    [Test]
    public async Task ShouldNotAuthenticate_WhenHeaderIsNotFound()
    {
        var userInfo = new UserInfo(false, "");
        var host = await BuildApiKeyAuthenticatedHost
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
        var apiKeyAuthOptions = host.Services.GetRequiredService<ApiKeyAuthenticationOptions>();
        apiKeyAuthOptions.Headers = new[]
        {
            new ApiKeyHeaderOptions
            {
                HeaderName = "XTI_API_KEY",
                ApiKeys = new []
                {
                    new ApiKeyOptions
                    {
                        ApiKey = "ApiKey1",
                        UserName = "User1"
                    }
                }
            }
        };
        var testServer = host.GetTestServer();
        testServer.BaseAddress = new Uri("https://localhost");
        var absoluteUrl = new Uri(testServer.BaseAddress, "Test");
        var requestBuilder = testServer.CreateRequest(absoluteUrl.ToString());
        requestBuilder.AddHeader("XTI_API_KEY1", "ApiKey1");
        var response = await requestBuilder.GetAsync();
        Assert.That(userInfo.IsAuthenticated, Is.False, "Should not authenticate when header is not found");
    }

    private static async Task<IHost> BuildApiKeyAuthenticatedHost(Func<HttpContext, Task> config)
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
                            services.AddSingleton<ApiKeyAuthenticationOptions>();
                            services.AddFakesForXtiWebApp();
                            services
                                .AddAuthentication(defaultScheme)
                                .AddApiKeyAuthentication(services)
                                .AddPolicyScheme
                                (
                                    defaultScheme,
                                    defaultScheme,
                                    options =>
                                    {
                                        options.ForwardDefaultSelector = (c) =>
                                        {
                                            return ApiKeyAuthentication.DefaultScheme;
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
