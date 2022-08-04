using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace XTI_WebApp.Extensions;

public static class BasicAuthentication
{
    public static readonly string DefaultScheme = "xti_basicAuth";
}

public static class BasicAuthenticationExtensions
{
    public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder)
    {
        builder.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
        (
            BasicAuthentication.DefaultScheme,
            options =>
            {
                var dbg = options.ForwardForbid;
            }
        );
        return builder;
    }

    public static bool IsBasicAuthentication(this HttpContext context) 
    {
        string authorization = context.Request.Headers[HeaderNames.Authorization];
        var creds = BasicAuthenticationCredentials.Parse(authorization);
        return !string.IsNullOrEmpty(creds.UserName);
    }
}
