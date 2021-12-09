using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Secrets;
using XTI_Core;
using XTI_Secrets.Extensions;
using XTI_TempLog;
using XTI_TempLog.Extensions;

namespace XTI_App.Extensions;

public static class AppExtensions
{
    public static void AddAppServices(this IServiceCollection services, IHostEnvironment hostEnv, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.Configure<AppOptions>(configuration.GetSection(AppOptions.App));
        services.AddSingleton<IClock, UtcClock>();
        services.AddSingleton<XtiFolder>();
        services.AddFileSecretCredentials(hostEnv);
        services.AddScoped(sp => sp.GetRequiredService<IXtiPathAccessor>().Value());
        services.AddScoped(sp => sp.GetRequiredService<XtiPath>().Version);
        services.AddScoped<IAppContext, CachedAppContext>();
        services.AddScoped<CachedUserContext>();
        services.AddScoped<IUserContext>(sp => sp.GetRequiredService<CachedUserContext>());
        services.AddScoped<ICachedUserContext>(sp => sp.GetRequiredService<CachedUserContext>());
        services.AddScoped<SystemUserCredentials>();
        services.AddScoped<ISystemUserCredentials>(sp =>
        {
            var cache = sp.GetRequiredService<IMemoryCache>();
            var sourceCredentials = sp.GetRequiredService<SystemUserCredentials>();
            return new CachedSystemUserCredentials(cache, sourceCredentials);
        });
        services.AddScoped<SystemUserContext>();
        services.AddScoped<ISystemUserContext, CachedSystemUserContext>();
        services.AddScoped<IAppApiUser, AppApiUser>();
        services.AddScoped(sp =>
        {
            var factory = sp.GetRequiredService<AppApiFactory>();
            var user = sp.GetRequiredService<IAppApiUser>();
            return factory.Create(user);
        });
        services.AddTempLogServices(configuration);
    }

    public static void AddThrottledLog<TAppApi>(this IServiceCollection services, Action<TAppApi, ThrottledLogsBuilder> action)
        where TAppApi : IAppApi
    {
        TempLogExtensions.AddThrottledLog
        (
            services,
            (sp, builder) =>
            {
                var api = (TAppApi)sp.GetRequiredService<AppApiFactory>().CreateForSuperUser();
                action(api, builder);
            }
        );
    }

    public static ThrottledPathBuilder Throttle(this ThrottledLogsBuilder builder, AppApiAction<EmptyRequest, EmptyActionResult> action)
        => builder.Throttle(action.Path.Format());

    public static ThrottledPathBuilder AndThrottle(this ThrottledPathBuilder builder, AppApiAction<EmptyRequest, EmptyActionResult> action)
        => builder.AndThrottle(action.Path.Format());
}