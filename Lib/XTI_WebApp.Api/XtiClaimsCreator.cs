using System.Security.Claims;
using XTI_TempLog.Abstractions;

namespace XTI_WebApp.Api;

public sealed class XtiClaimsCreator
{
    private readonly string sessionKey;
    private readonly AppUserName userName;

    public XtiClaimsCreator(SessionKey sessionKey, AppUserName userName)
    {
        this.sessionKey = sessionKey.Format();
        this.userName = userName;
    }

    public Claim[] Values() =>
    [
        new Claim(ClaimTypes.NameIdentifier, userName.Value),
        new Claim("UserName", userName.Value),
        new Claim("SessionKey", sessionKey)
    ];
}