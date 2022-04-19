using XTI_App.Abstractions;
using XTI_App.Secrets;

namespace XTI_App.Extensions;

public sealed class SystemCurrentUserName : ICurrentUserName
{
    private readonly ISystemUserCredentials systemUserCredentials;
    private AppUserName? cachedUserName;

    public SystemCurrentUserName(ISystemUserCredentials systemUserCredentials)
    {
        this.systemUserCredentials = systemUserCredentials;
    }

    public async Task<AppUserName> Value()
    {
        if(cachedUserName == null)
        {
            var credentials = await systemUserCredentials.Value();
            cachedUserName = new AppUserName(credentials.UserName);
        }
        return cachedUserName;
    }
}
