using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Secrets;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_Secrets.Extensions;
using XTI_TempLog;
using XTI_TempLog.Extensions;

namespace XTI_App.Extensions;

public static class AppExtensions
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddSingleton<XtiFolder>();
        services.AddSingleton(sp => XtiEnvironment.Parse(sp.GetRequiredService<IHostEnvironment>().EnvironmentName));
        services.AddConfigurationOptions<AppOptions>(AppOptions.App);
        services.AddSingleton<IClock, UtcClock>();
        services.AddFileSecretCredentials();
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
        services.AddTempLogServices();
    }

    public static void AddThrottledLog<TAppApi>(this IServiceCollection services, Action<TAppApi, ThrottledLogsBuilder> action)
        where TAppApi : IAppApi
    {
        TempLogExtensions.AddThrottledLog
        (
            services,
            (sp, builder) =>
            {
                using var scope = sp.CreateScope();
                var api = (TAppApi)scope.ServiceProvider.GetRequiredService<AppApiFactory>().CreateForSuperUser();
                action(api, builder);
            }
        );
    }

    public static ThrottledPathBuilder Throttle(this ThrottledLogsBuilder builder, IAppApiAction action)
        => builder.Throttle(action.Path.Format());

    public static ThrottledPathBuilder AndThrottle(this ThrottledPathBuilder builder, IAppApiAction action)
        => builder.AndThrottle(action.Path.Format());
}