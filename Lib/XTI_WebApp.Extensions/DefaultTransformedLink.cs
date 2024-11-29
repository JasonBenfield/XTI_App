using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

internal sealed class DefaultTransformedLink : ITransformedLink
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly XtiBasePath xtiBasePath;
    private readonly IUserContext userContext;
    private readonly LinkModel link;

    public DefaultTransformedLink(IHttpContextAccessor httpContextAccessor, XtiBasePath xtiBasePath, IUserContext userContext, LinkModel link)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.xtiBasePath = xtiBasePath;
        this.userContext = userContext;
        this.link = link;
    }

    public async Task<LinkModel> Value()
    {
        var l = link;
        if (link.IsXtiPath() && httpContextAccessor.HttpContext != null)
        {
            var url = link.Url.Replace("~", $"/{xtiBasePath.Value.App}/{AppVersionKey.Current.DisplayText}");
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
