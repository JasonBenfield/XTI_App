using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using XTI_App.Abstractions;
using XTI_App.Extensions;
using XTI_App.Secrets;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_TempLog;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public static class WebAppExtensions
{
    public static void UseXtiDefaults(this WebApplication app)
    {
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseResponseCaching();
        app.UseXti();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute
            (
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
        });
    }

    public static void AddWebAppServices(this IServiceCollection services)
    {
        services.Configure<RazorViewEngineOptions>(o =>
        {
            o.ViewLocationFormats.Add("/Views/Exports/Shared/{1}/{0}" + RazorViewEngine.ViewExtension);
            o.ViewLocationFormats.Add("/Views/Exports/Shared/{0}" + RazorViewEngine.ViewExtension);
        });
        services.AddHttpContextAccessor();
        services.AddConfigurationOptions<WebAppOptions>(WebAppOptions.WebApp);
        services.AddConfigurationOptions<XtiAuthenticationOptions>(XtiAuthenticationOptions.XtiAuthentication);
        services.AddScoped<ILogoutProcess, LogoutProcess>();
        services.AddScoped<LogoutAction>();
        services.AddScoped<GetUserAccessAction>();
        services.AddScoped<GetMenuLinksAction>();
        services.AddScoped<UserProfileAction>();
        services.AddScoped<CacheBust>();
        services.AddScoped<IPageContext, PageContext>();
        services.AddScoped<WebViewResultFactory>();
        services.AddScoped(sp => sp.GetRequiredService<XtiPath>().Version);
        services.AddScoped(sp =>
        {
            var xtiFolder = sp.GetRequiredService<XtiFolder>();
            var appKey = sp.GetRequiredService<AppKey>();
            return xtiFolder.AppDataFolder(appKey);
        });
        services.AddScoped<CurrentSession>();
        services.AddScoped<XtiRequestContext>();
        services.AddScoped<IAnonClient>(sp =>
        {
            var dataProtector = sp.GetDataProtector(new[] { "XTI_Apps_Anon" });
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            return new AnonClient(dataProtector, httpContextAccessor);
        });
        services.AddScoped<IXtiPathAccessor, WebXtiPathAccessor>();
        services.AddSingleton<ISystemUserCredentials, SystemUserCredentials>();
        services.AddSingleton<SystemCurrentUserName>();
        services.AddScoped<ICurrentUserName, WebCurrentUserName>();
        services.AddScoped<IAppEnvironmentContext, WebAppEnvironmentContext>();
        services.AddSingleton<SelfAppClientDomain>();
        services.AddScoped<LoginUrl>();
        services.AddScoped<IIncludedLinkFactory, DefaultIncludedLinkFactory>();
        services.AddScoped<ITransformedLinkFactory, DefaultTransformedLinkFactory>();
        services.AddScoped<UserMenuDefinition>();
        services.AddScoped<IMenuDefinitionBuilder, DefaultMenuDefinitionBuilder>();
        services.AddScoped(sp => sp.GetRequiredService<IMenuDefinitionBuilder>().Build());
    }

    public static void SetDefaultJsonOptions(this JsonOptions options)
    {
        options.JsonSerializerOptions.AddCoreConverters();
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    }

    public static void SetDefaultMvcOptions(this MvcOptions options)
    {
        options.CacheProfiles.Add("Default", new CacheProfile
        {
            Duration = 2592000,
            Location = ResponseCacheLocation.Any,
            NoStore = false
        });
        options.ModelBinderProviders.Insert(0, new FormModelBinderProvider());
    }
}