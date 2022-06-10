using System.Security.Claims;
using XTI_App.Abstractions;

namespace XTI_WebApp.Api;

public sealed class XtiClaimsCreator
{
    private readonly string sessionKey;
    private readonly AppUserName userName;

    public XtiClaimsCreator(string sessionKey, AppUserName userName)
    {
        this.sessionKey = sessionKey;
        this.userName = userName;
    }

    public IEnumerable<Claim> Values() => new[]
    {
        new Claim(ClaimTypes.NameIdentifier, userName.Value),
        new Claim("UserName", userName.Value),
        new Claim("SessionKey", sessionKey)
    };
}