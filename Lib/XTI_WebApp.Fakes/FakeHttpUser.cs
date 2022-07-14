using System.Security.Claims;
using XTI_App.Abstractions;
using XTI_WebApp.Api;

namespace XTI_WebApp.Fakes;

public sealed class FakeHttpUser
{
    public ClaimsPrincipal Create() => new ClaimsPrincipal();

    public ClaimsPrincipal Create(string sessionKey, UserContextModel userContextModel)
    {
        var claims = new XtiClaimsCreator(sessionKey, userContextModel.User.UserName).Values();
        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }
}