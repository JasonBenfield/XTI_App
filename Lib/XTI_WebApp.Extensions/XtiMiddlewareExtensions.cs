using Microsoft.AspNetCore.Builder;

namespace XTI_WebApp.Extensions;

public static class XtiMiddlewareExtensions
{
    public static IApplicationBuilder UseXti(this IApplicationBuilder builder) =>
        builder
            .UseMiddleware<ErrorHandlingMiddleware>()
            .UseMiddleware<SessionLogMiddleware>();
}