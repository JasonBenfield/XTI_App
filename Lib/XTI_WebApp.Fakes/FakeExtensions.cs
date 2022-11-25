using Microsoft.Extensions.DependencyInjection;
using XTI_App.Fakes;
using XTI_Core.Extensions;
using XTI_TempLog;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;
using XTI_WebApp.Extensions;

namespace XTI_WebApp.Fakes;

public static class FakeExtensions
{
    public static void AddFakesForXtiWebApp(this IServiceCollection services)
    {
        services.AddFakesForXtiApp();
        services.AddHttpContextAccessor();
        services.AddConfigurationOptions<WebAppOptions>(WebAppOptions.WebApp);
        services.AddScoped<IAnonClient, FakeAnonClient>();
        services.AddScoped<CacheBust>();
        services.AddScoped<IPageContext, PageContext>();
        services.AddScoped<IAppEnvironmentContext, WebAppEnvironmentContext>();
        services.AddScoped<CurrentSession>();
        services.AddScoped<ILogoutProcess, LogoutProcess>();
        services.AddScoped<LogoutAction>();
        services.AddScoped<ILoginReturnKey, FakeLoginReturnKey>();
        services.AddScoped<LoginUrl>();
        services.AddScoped<IIncludedLinkFactory, DefaultIncludedLinkFactory>();
        services.AddScoped<FakeTransformedLinkFactory>();
        services.AddScoped<ITransformedLinkFactory>(sp => sp.GetRequiredService<FakeTransformedLinkFactory>());
        services.AddScoped<FakeLinkService>();
        services.AddScoped<ILinkService>(sp=>sp.GetRequiredService<FakeLinkService>());
    }
}