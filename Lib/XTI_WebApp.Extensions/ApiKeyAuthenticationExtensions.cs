using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using XTI_Core.Extensions;

namespace XTI_WebApp.Extensions;

public static class ApiKeyAuthentication
{
    public static readonly string DefaultScheme = "ApiKeyAuth";
}

public static class ApiKeyAuthenticationExtensions
{
    public static AuthenticationBuilder AddApiKeyAuthentication(this AuthenticationBuilder builder, IServiceCollection services)
    {
        services.AddConfigurationOptions<ApiKeyAuthenticationOptions>(ApiKeyAuthenticationOptions.ApiKeyAuth);
        builder.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>
        (
            ApiKeyAuthentication.DefaultScheme,
            options =>
            {
                var dbg = options.ForwardForbid;
            }
        );
        return builder;
    }

    public static bool IsApiKeyAuthentication(this HttpContext context) =>
        !string.IsNullOrWhiteSpace(ApiKeyHeaderName(context));

    public static string? ApiKeyHeaderName(this HttpContext context)
    {
        var options = context.RequestServices.GetService<ApiKeyAuthenticationOptions>();
        if(options != null)
        {
            var authHeaderNames = options.Headers.Select(h => h.HeaderName).ToList();
            var reqHeaderNames = context.Request.Headers
                .Select(h => h.Key)
                .ToArray()
                ?? new string[0];
            return authHeaderNames.Intersect(reqHeaderNames).FirstOrDefault();
        }
        return null;
    }
}
