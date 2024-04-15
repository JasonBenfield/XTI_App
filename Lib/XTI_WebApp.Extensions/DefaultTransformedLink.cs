using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

internal sealed class DefaultTransformedLink : ITransformedLink
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IXtiPathAccessor xtiPathAccessor;
    private readonly IUserContext userContext;
    private readonly LinkModel link;

    public DefaultTransformedLink(IHttpContextAccessor httpContextAccessor, IXtiPathAccessor xtiPathAccessor, IUserContext userContext, LinkModel link)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.xtiPathAccessor = xtiPathAccessor;
        this.userContext = userContext;
        this.link = link;
    }

    public async Task<LinkModel> Value()
    {
        var l = link;
        if (link.IsXtiPath() && httpContextAccessor.HttpContext != null)
        {
            var xtiPath = xtiPathAccessor.Value();
            var url = link.Url.Replace("~", $"/{xtiPath.App}/{AppVersionKey.Current.DisplayText}");
            l = l with { Url = url };
        }
        if (l.DisplayText == "{User.FullName}")
        {
            var user = await userContext.User();
            l = l with { DisplayText = user.Name.DisplayText };
        }
        return l;
    }
}
