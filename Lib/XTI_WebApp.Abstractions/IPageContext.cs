namespace XTI_WebApp.Abstractions;

public interface IPageContext
{
    string CacheBust { get; }
    string EnvironmentName { get; }
    string AppTitle { get; }
    bool IsAuthenticated { get; }
    string UserName { get; }
    string PageTitle { get; set; }
    string PageName { get; set; }
    public AppVersionDomain[] WebAppDomains { get; }

    Task<string> Serialize();
}