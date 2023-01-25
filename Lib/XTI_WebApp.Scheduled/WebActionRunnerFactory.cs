using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_App.Hosting;
using XTI_Core;
using XTI_TempLog;

namespace XTI_WebApp.Scheduled;

public sealed class WebActionRunnerFactory : IActionRunnerFactory
{
    private readonly IServiceProvider services;

    public WebActionRunnerFactory(IServiceProvider services)
    {
        this.services = services;
    }

    public IAppApi CreateAppApi()
    {
        var apiFactory = services.GetRequiredService<AppApiFactory>();
        var userContext = services.GetRequiredService<IUserContext>();
        var appContext = services.GetRequiredService<IAppContext>();
        var systemCurrentUserName = services.GetRequiredService<SystemCurrentUserName>();
        var currentUserAccess = new CurrentUserAccess(userContext, appContext, systemCurrentUserName);
        var pathAccessor = services.GetRequiredService<ActionRunnerXtiPathAccessor>();
        var apiUser = new AppApiUser(currentUserAccess, pathAccessor);
        return apiFactory.Create(apiUser);
    }

    public TempLogSession CreateTempLogSession()
    {
        var tempLog = services.GetRequiredService<TempLog>();
        var appEnvContext = services.GetRequiredService<ScheduledAppEnvironmentContext>();
        var cache = services.GetRequiredService<IMemoryCache>();
        if (!cache.TryGetValue<CurrentSession>("scheduled_currentSession", out var currentSession))
        {
            currentSession = new CurrentSession();
            cache.Set("scheduled_currentSession", currentSession);
        }
        var clock = services.GetRequiredService<IClock>();
        var throttleLogs = services.GetRequiredService<ThrottledLogs>();
        return new TempLogSession(tempLog, appEnvContext, currentSession ?? new CurrentSession(), clock, throttleLogs);
    }
}