using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Text;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public static class CookieAndTokenAuthentication
{
    public static bool IsBearerAuthentication(this HttpContext context)
    {
        string? authorization = context.Request.Headers[HeaderNames.Authorization];
        return !string.IsNullOrWhiteSpace(authorization) && authorization.StartsWith("Bearer ");
    }

    public static void ConfigureXtiCookieAndTokenAuthentication(this IServiceCollection services, XtiEnvironment xtiEnv, IConfiguration config)
    {
        const string defaultScheme = "JWT_OR_COOKIE";
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = defaultScheme;
                options.DefaultChallengeScheme = defaultScheme;
            })
            .AddCookie("Cookies", options => options.SetXtiCookieOptions(xtiEnv, config))
            .AddJwtBearer("Bearer", options => options.SetXtiJwtBearerOptions(config))
            .AddPolicyScheme
            (
                defaultScheme,
                defaultScheme,
                options =>
                {
                    options.ForwardDefaultSelector = context =>
                        context.IsBearerAuthentication()
                            ? "Bearer"
                            : "Cookies";
                }
            );
    }

    public static void SetXtiJwtBearerOptions(this JwtBearerOptions options, IConfiguration config)
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = CreateTokenValidationParameters(config);
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = c =>
            {
                return Task.CompletedTask;
            },
            OnChallenge = c =>
            {
                return Task.CompletedTask;
            }
        };
    }

    public static void SetXtiCookieOptions(this CookieAuthenticationOptions options, XtiEnvironment xtiEnv, IConfiguration config)
    {
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
        options.Cookie.Path = "/";
        var webAppOptions = config.Get<DefaultWebAppOptions>() ?? new();
        if (!string.IsNullOrWhiteSpace(webAppOptions.XtiAuthentication.CookieName))
        {
            options.Cookie.Name = webAppOptions.XtiAuthentication.CookieName;
        }
        if (!string.IsNullOrWhiteSpace(webAppOptions.XtiAuthentication.CookieDomain))
        {
            options.Cookie.Domain = webAppOptions.XtiAuthentication.CookieDomain;
        }
        options.TicketDataFormat = CreateAuthTicketFormat(xtiEnv, options.DataProtectionProvider, config);
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = async (x) =>
            {
                if (x.Request.IsApiRequest())
                {
                    if (x.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
                    {
                        x.Response.StatusCode = StatusCodes.Status403Forbidden;
                    }
                    else
                    {
                        x.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    }
                }
                else
                {
                    await RedirectToLogin(x.HttpContext.RequestServices, x);
                }
            }
        };
        options.ReturnUrlParameter = "returnUrl";
    }

    private static TokenValidationParameters CreateTokenValidationParameters(IConfiguration config)
    {
        var options = config.Get<DefaultWebAppOptions>();
        var key = Encoding.ASCII.GetBytes(options?.XtiAuthentication?.JwtSecret ?? "");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        return tokenValidationParameters;
    }

    private static JwtAuthTicketFormat CreateAuthTicketFormat(XtiEnvironment xtiEnv, IDataProtectionProvider? dataProtectionProvider, IConfiguration config)
    {
        var options = config.Get<DefaultWebAppOptions>() ?? new DefaultWebAppOptions();
        var key = Encoding.ASCII.GetBytes(options.XtiAuthentication.JwtSecret);
        var dataSerializer = new TicketSerializer();
        if (dataProtectionProvider == null)
        {
            var xtiFolder = new XtiFolder(xtiEnv);
            var keyDirPath = xtiFolder.SharedAppDataFolder()
                .WithSubFolder("Keys")
                .Path();
            dataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(keyDirPath));
        }
        var dataProtector = dataProtectionProvider.CreateProtector(new[] { "XTI_Apps_Auth1" });
        var authTicketFormat = new JwtAuthTicketFormat
        (
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            },
            dataSerializer,
            dataProtector
        );
        return authTicketFormat;
    }

    private static async Task RedirectToLogin(IServiceProvider sp, RedirectContext<CookieAuthenticationOptions> x)
    {
        var returnUrl = $"{x.Request.Scheme}://{x.Request.Host.Value}{x.Request.PathBase.Value}";
        if (x.Request.Path.HasValue)
        {
            if (!x.Request.Path.Value.StartsWith("/"))
            {
                returnUrl += "/";
            }
            returnUrl += x.Request.Path.Value;
        }
        if (x.Request.QueryString.HasValue)
        {
            returnUrl += $"{x.Request.QueryString.Value}";
        }
        var loginUrl = await sp.GetRequiredService<LoginUrl>().Value(returnUrl);
        x.Response.Redirect(loginUrl);
    }

    private static bool IsApiRequest(this HttpRequest request) =>
        request != null &&
        request.Method == "POST" &&
        request.ContentType?.StartsWith(WebContentTypes.Json, StringComparison.OrdinalIgnoreCase) == true;
}