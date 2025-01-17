﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;
using XTI_Core.Extensions;
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
    ) => config.UseXtiConfiguration(hostEnv, appKey, []);

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
    ) => config.UseXtiConfiguration(environment, appKey, []);

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
        services.AddSingleton<AppVersionKeyAccessor>();
        services.AddSingleton(sp => sp.GetRequiredService<AppVersionKeyAccessor>().Value);
        services.AddSingleton<XtiBasePath>();
        services.AddSingleton<DefaultModifierKeyAccessor>();
        services.AddSingleton<IModifierKeyAccessor>(sp => sp.GetRequiredService<DefaultModifierKeyAccessor>());
        services.AddSingleton<IClock, LocalClock>();
        services.AddConfigurationOptions<DefaultAppOptions>();
        services.AddSingleton(sp => sp.GetRequiredService<DefaultAppOptions>().HubClient);
        services.AddSingleton(sp => sp.GetRequiredService<DefaultAppOptions>().XtiToken);
        services.AddSingleton(sp => sp.GetRequiredService<DefaultAppOptions>().DB);
        services.AddSingleton(sp => sp.GetRequiredService<DefaultAppOptions>().TempLog);
        services.AddScoped<CachedAppContext>();
        services.AddScoped<IAppContext>(sp => sp.GetRequiredService<CachedAppContext>());
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
        services.AddSingleton
        (
            sp =>
            {
                var appKey = sp.GetRequiredService<AppKey>().Format().Replace(" ", "");
                return new TempLogRepository
                (
                    sp.GetRequiredService<TempLog>(),
                    $"{appKey}_{Environment.ProcessId:0000000000}".ToLower()
                );
            }
        );
        services.AddScoped<IAppClientSessionKey, DefaultAppClientSessionKey>();
        services.AddScoped<IAppClientRequestKey, DefaultAppClientRequestKey>();
        services.AddScoped<AppClientOptions>();
        services.AddSingleton<InstallationIDAccessor, FileInstallationIDAccessor>();
    }

    public static void AddThrottledLog(this IServiceCollection services, Action<IAppApi, ThrottledLogsBuilder> action)
    {
        TempLogExtensions.AddThrottledLog
        (
            services,
            (sp, builder) =>
            {
                using var scope = sp.CreateScope();
                var api = scope.ServiceProvider.GetRequiredService<AppApiFactory>().CreateForSuperUser();
                var xtiBasePath = scope.ServiceProvider.GetRequiredService<XtiBasePath>();
                var throttledLogPaths = api.ThrottledLogPaths(xtiBasePath);
                builder.ApplyThrottledPaths(throttledLogPaths);
                action(api, builder);
            }
        );
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
                var xtiBasePath = scope.ServiceProvider.GetRequiredService<XtiBasePath>();
                var throttledLogPaths = api.ThrottledLogPaths(xtiBasePath);
                builder.ApplyThrottledPaths(throttledLogPaths);
                action(api, builder);
            }
        );
    }

    public static ThrottledLogPathBuilder Throttle(this ThrottledLogsBuilder builder, IAppApiAction action) =>
        builder.Throttle($"/{action.Path.Group.DisplayText}/{action.Path.Action.DisplayText}".Replace(" ", ""));

    public static ThrottledLogPathBuilder AndThrottle(this ThrottledLogPathBuilder builder, IAppApiAction action) =>
        builder.AndThrottle($"/{action.Path.Group.DisplayText}/{action.Path.Action.DisplayText}".Replace(" ", ""));
}