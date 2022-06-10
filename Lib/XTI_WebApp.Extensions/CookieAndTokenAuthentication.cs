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
using XTI_Core;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public static class CookieAndTokenAuthentication
{
    public static bool IsBearerAuthentication(this HttpContext context)
    {
        string authorization = context.Request.Headers[HeaderNames.Authorization];
        return !string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer ");
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
        options.TokenValidationParameters = createTokenValidationParameters(config);
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
        var xtiAuthOptions = config.GetSection(XtiAuthenticationOptions.XtiAuthentication).Get<XtiAuthenticationOptions>();
        if (!string.IsNullOrWhiteSpace(xtiAuthOptions?.CookieName))
        {
            options.Cookie.Name = xtiAuthOptions.CookieName;
        }
        if (!string.IsNullOrWhiteSpace(xtiAuthOptions?.CookieDomain))
        {
            options.Cookie.Domain = xtiAuthOptions.CookieDomain;
        }
        options.TicketDataFormat = createAuthTicketFormat(xtiEnv, options.DataProtectionProvider, config);
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
                    await redirectToLogin(x.HttpContext.RequestServices, x);
                }
            }
        };
        options.ReturnUrlParameter = "returnUrl";
    }

    private static TokenValidationParameters createTokenValidationParameters(IConfiguration config)
    {
        var xtiAuthOptions = config.GetSection(XtiAuthenticationOptions.XtiAuthentication).Get<XtiAuthenticationOptions>();
        var key = Encoding.ASCII.GetBytes(xtiAuthOptions.JwtSecret);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        return tokenValidationParameters;
    }

    private static JwtAuthTicketFormat createAuthTicketFormat(XtiEnvironment xtiEnv, IDataProtectionProvider? dataProtectionProvider, IConfiguration config)
    {
        var xtiAuthOptions = config.GetSection(XtiAuthenticationOptions.XtiAuthentication).Get<XtiAuthenticationOptions>() ?? new XtiAuthenticationOptions();
        var key = Encoding.ASCII.GetBytes(xtiAuthOptions.JwtSecret);
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

    private static async Task redirectToLogin(IServiceProvider sp, RedirectContext<CookieAuthenticationOptions> x)
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

    private static bool IsApiRequest(this HttpRequest request)
        => request != null
            && request.Method == "POST"
            && request.ContentType?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) == true;
}