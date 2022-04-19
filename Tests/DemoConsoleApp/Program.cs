using DemoConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_App.Fakes;
using XTI_App.Hosting;
using XTI_ConsoleApp;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_TempLog;
using XTI_TempLog.Fakes;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.UseXtiConfiguration(hostingContext.HostingEnvironment, DemoInfo.AppKey.Name.DisplayText, DemoInfo.AppKey.Type.DisplayText, args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddAppServices();
        services.AddScoped(sp =>
        {
            var xtiFolder = sp.GetRequiredService<XtiFolder>();
            var appKey = sp.GetRequiredService<AppKey>();
            return xtiFolder.AppDataFolder(appKey);
        });
        services.AddSingleton<CurrentSession>();
        services.AddScoped<ActionRunnerXtiPathAccessor>();
        services.AddScoped<IXtiPathAccessor>(sp => sp.GetRequiredService<ActionRunnerXtiPathAccessor>());
        services.AddScoped<IActionRunnerFactory, ActionRunnerFactory>();
        services.AddSingleton<ICurrentUserName>(_ => new FakeCurrentUserName(new AppUserName("test.user")));
        services.AddSingleton<IAppEnvironmentContext, AppEnvironmentContext>();
        services.AddHostedService<AppAgendaHostedService>();
        services.AddAppAgenda
        (
            (sp, b) =>
            {
                b.AddImmediate<DemoAppApi>(api => api.Demo.First);
                b.AddImmediate<DemoAppApi>(api => api.Demo.Second);
                b.AddImmediate<DemoAppApi>(api => api.Demo.Third);
                var api = sp.GetRequiredService<IAppApi>();
                if (api is ConsoleAppApiWrapper)
                {
                    b.AddImmediate(new ResourceGroupName("Lifetime"), new ResourceName("StopApplication"));
                }
            }
        );
        services.AddSingleton(_ => DemoInfo.AppKey);
        services.AddScoped<FakeUserContext>();
        services.AddScoped<ISourceUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
        services.AddScoped<IUserContext>(sp => sp.GetRequiredService<ISourceUserContext>());
        services.AddScoped<FakeAppContext>();
        services.AddScoped<ISourceAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
        services.AddScoped<IAppContext>(sp => sp.GetRequiredService<ISourceAppContext>());
        var service = services.First(s => s.ServiceType == typeof(IAppEnvironmentContext));
        services.Remove(service);
        services.AddSingleton<IAppEnvironmentContext, FakeAppEnvironmentContext>();
        services.AddScoped<AppApiFactory, DemoAppApiFactory>();
        services.AddScoped(sp => sp.GetRequiredService<AppApiFactory>().CreateForSuperUser());
    })
    .RunConsoleAsync();