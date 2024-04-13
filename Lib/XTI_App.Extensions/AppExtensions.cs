using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;
using XTI_TempLog;
using XTI_TempLog.Extensions;

namespace XTI_App.Extensions;

public static class AppExtensions
{
    public static IConfigurationBuilder UseXtiConfiguration
    (
        this IConfigurationBuilder config,
        IHostEnvironment hostEnv,
        AppKey appKey
    ) => config.UseXtiConfiguration(hostEnv, appKey, new string[0]);

    public static IConfigurationBuilder UseXtiConfiguration
    (
        this IConfigurationBuilder config,
        IHostEnvironment hostEnv,
        AppKey appKey,
        string[] args
    ) =>
        config.UseXtiConfiguration
        (
            XtiEnvironment.Parse(hostEnv.EnvironmentName),
            appKey,
            args
        );

    public static IConfigurationBuilder UseXtiConfiguration
    (
        this IConfigurationBuilder config, 
        XtiEnvironment environment, 
        AppKey appKey
    ) => config.UseXtiConfiguration(environment, appKey, new string[0]);

    public static IConfigurationBuilder UseXtiConfiguration
    (
        this IConfigurationBuilder config, 
        XtiEnvironment xtiEnv, 
        AppKey appKey, 
        string[] args
    ) => 
        XTI_Core.Extensions.ConfigurationExtensions.UseXtiConfiguration
        (
            config, 
            xtiEnv,
            appKey.Name.DisplayText, 
            appKey.Type.DisplayText, 
            args
        );

    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddSingleton<XtiFolder>();
        services.AddSingleton(sp => XtiEnvironment.Parse(sp.GetRequiredService<IHostEnvironment>().EnvironmentName));
        services.AddSingleton<IClock, UtcClock>();
        services.AddScoped(sp => sp.GetRequiredService<IXtiPathAccessor>().Value());
        services.AddScoped(sp => sp.GetRequiredService<XtiPath>().Version);
        services.AddScoped<IAppContext, CachedAppContext>();
        services.AddScoped<CachedUserContext>();
        services.AddScoped<IUserContext>(sp => sp.GetRequiredService<CachedUserContext>());
        services.AddScoped<ICachedUserContext>(sp => sp.GetRequiredService<CachedUserContext>());
        services.AddScoped<CurrentUserAccess>();
        services.AddScoped<IAppApiUser, AppApiUser>();
        services.AddScoped(sp =>
        {
            var factory = sp.GetRequiredService<AppApiFactory>();
            var user = sp.GetRequiredService<IAppApiUser>();
            return factory.Create(user);
        });
        services.AddTempLogServices();
        services.AddScoped<IAppClientRequestKey, DefaultAppClientRequestKey>();
        services.AddSingleton<InstallationIDAccessor, FileInstallationIDAccessor>();
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
        => builder.Throttle($"/{action.Path.Group.DisplayText}/{action.Path.Action.DisplayText}".Replace(" ", ""));

    public static ThrottledPathBuilder AndThrottle(this ThrottledPathBuilder builder, IAppApiAction action)
        => builder.AndThrottle($"/{action.Path.Group.DisplayText}/{action.Path.Action.DisplayText}".Replace(" ", ""));
}