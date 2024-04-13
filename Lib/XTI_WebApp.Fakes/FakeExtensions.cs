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
        services.AddConfigurationOptions<DefaultWebAppOptions>();
        services.AddScoped<IAnonClient, FakeAnonClient>();
        services.AddScoped<CacheBust>();
        services.AddScoped<IPageContext, PageContext>();
        services.AddScoped<WebViewResultFactory>();
        services.AddScoped<IAppEnvironmentContext, WebAppEnvironmentContext>();
        services.AddScoped<CurrentSession>();
        services.AddScoped<ILogoutProcess, LogoutProcess>();
        services.AddScoped<LogoutAction>();
        services.AddScoped<ILoginReturnKey, FakeLoginReturnKey>();
        services.AddScoped<LoginUrl>();
        services.AddScoped<GetMenuLinksAction>();
        services.AddScoped<IIncludedLinkFactory, DefaultIncludedLinkFactory>();
        services.AddScoped<FakeTransformedLinkFactory>();
        services.AddScoped<ITransformedLinkFactory>(sp => sp.GetRequiredService<FakeTransformedLinkFactory>());
        services.AddSingleton<UserMenuDefinition>();
        services.AddSingleton<IMenuDefinitionBuilder, DefaultMenuDefinitionBuilder>();
        services.AddSingleton(sp => sp.GetRequiredService<IMenuDefinitionBuilder>().Build());
    }
}