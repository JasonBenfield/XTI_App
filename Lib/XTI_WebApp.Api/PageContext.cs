using System.Text.Json;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;
using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class PageContext : IPageContext
{
    private readonly AppClients appClients;
    private readonly CacheBust cacheBust;
    private readonly IAppContext appContext;
    private readonly ICurrentUserName currentUserName;
    private readonly XtiEnvironment xtiEnv;

    public PageContext(AppClients appClients, CacheBust cacheBust, IAppContext appContext, ICurrentUserName currentUserName, XtiEnvironment xtiEnv)
    {
        this.appClients = appClients;
        this.cacheBust = cacheBust;
        this.appContext = appContext;
        this.currentUserName = currentUserName;
        this.xtiEnv = xtiEnv;
    }

    public string CacheBust { get; private set; } = "";
    public string EnvironmentName { get; private set; } = "";
    public bool IsAuthenticated { get; private set; } = false;
    public string UserName { get; private set; } = "";
    public string AppTitle { get; private set; } = "";
    public string PageTitle { get; set; } = "";
    public string PageName { get; set; } = "";
    public AppVersionDomain[] WebAppDomains { get; private set; } = new AppVersionDomain[0];

    public async Task<string> Serialize()
    {
        CacheBust = await cacheBust.Value();
        var app = await appContext.App();
        AppTitle = app.Title;
        EnvironmentName = xtiEnv.EnvironmentName;
        var userName = await currentUserName.Value();
        if (userName.Equals(AppUserName.Anon))
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
        return JsonSerializer.Serialize(this);
    }
}