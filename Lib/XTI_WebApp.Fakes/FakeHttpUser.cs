using System.Security.Claims;
using XTI_App.Abstractions;
using XTI_TempLog.Abstractions;
using XTI_WebApp.Api;

namespace XTI_WebApp.Fakes;

public sealed class FakeHttpUser
{
    public ClaimsPrincipal Create() => new();

    public ClaimsPrincipal Create(SessionKey sessionKey, AppUserModel user)
    {
        var claims = new XtiClaimsCreator(sessionKey, user.UserName).Values();
        var identity = new ClaimsIdentity(claims, "Test");
        return new(identity);
    }
}