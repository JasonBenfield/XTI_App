using Microsoft.AspNetCore.Http;
using System.Text.Json;
using XTI_App.Api;
using XTI_Core;
using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class PageContext : IPageContext
{
    private readonly AppClients appClients;
    private readonly CacheBust cacheBust;
    private readonly IAppContext appContext;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ICurrentUserName currentUserName;
    private readonly XtiEnvironment xtiEnv;
    private bool hasLoaded = false;

    public PageContext(AppClients appClients, CacheBust cacheBust, IAppContext appContext, IHttpContextAccessor httpContextAccessor, ICurrentUserName currentUserName, XtiEnvironment xtiEnv)
    {
        this.appClients = appClients;
        this.cacheBust = cacheBust;
        this.appContext = appContext;
        this.httpContextAccessor = httpContextAccessor;
        this.currentUserName = currentUserName;
        this.xtiEnv = xtiEnv;
    }

    public string CacheBust { get; private set; } = "";
    public string EnvironmentName { get; private set; } = "";
    public bool IsAuthenticated { get; private set; } = false;
    public string RootUrl { get; private set; } = "";
    public string UserName { get; private set; } = "";
    public string AppTitle { get; private set; } = "";
    public string PageTitle { get; set; } = "";
    public string PageName { get; set; } = "";
    public AppVersionDomain[] WebAppDomains { get; private set; } = new AppVersionDomain[0];

    public async Task<string> Serialize()
    {
        if (!hasLoaded)
        {
            CacheBust = await cacheBust.Value();
            var app = await appContext.App();
            AppTitle = app.App.AppKey.Name.DisplayText;
            EnvironmentName = xtiEnv.EnvironmentName;
            RootUrl = httpContextAccessor.HttpContext?.Request.PathBase ?? "";
            var userName = await currentUserName.Value();
            if (userName.IsAnon())
            {
                IsAuthenticated = false;
                UserName = "";
            }
            else
            {
                IsAuthenticated = true;
                UserName = userName.Value;
            }
            WebAppDomains = await appClients.Domains();
            hasLoaded = true;
        }
        return JsonSerializer.Serialize(this);
    }
}