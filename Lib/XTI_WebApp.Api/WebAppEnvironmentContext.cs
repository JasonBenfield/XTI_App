using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;
using XTI_TempLog;
using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class WebAppEnvironmentContext : IAppEnvironmentContext
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IAnonClient anonClient;
    private readonly ICurrentUserName currentUserName;
    private readonly InstallationIDAccessor installationIDAccessor;

    private AppEnvironment? value;

    public WebAppEnvironmentContext(IHttpContextAccessor httpContextAccessor, IAnonClient anonClient, ICurrentUserName currentUserName, InstallationIDAccessor installationIDAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.anonClient = anonClient;
        this.currentUserName = currentUserName;
        this.installationIDAccessor = installationIDAccessor;
    }

    public async Task<AppEnvironment> Value()
    {
        if (value == null)
        {
            anonClient.Load();
            var user = await currentUserName.Value();
            var requesterKey = anonClient.RequesterKey;
            if (string.IsNullOrWhiteSpace(requesterKey))
            {
                requesterKey = Guid.NewGuid().ToString("N");
            }
            var userAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "";
            var remoteAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";
            var installationID = await installationIDAccessor.Value();
            value = new AppEnvironment
            (
                user.Value,
                requesterKey,
                remoteAddress,
                userAgent,
                installationID
            );
        }
        return value;
    }
}