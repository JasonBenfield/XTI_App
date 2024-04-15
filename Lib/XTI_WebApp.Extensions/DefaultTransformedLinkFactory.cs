using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class DefaultTransformedLinkFactory : ITransformedLinkFactory
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IXtiPathAccessor xtiPathAccessor;
    private readonly IUserContext userContext;

    public DefaultTransformedLinkFactory(IHttpContextAccessor httpContextAccessor, IXtiPathAccessor xtiPathAccessor, IUserContext userContext)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.xtiPathAccessor = xtiPathAccessor;
        this.userContext = userContext;
    }

    public ITransformedLink Create(LinkModel link) => 
        new DefaultTransformedLink(httpContextAccessor, xtiPathAccessor, userContext, link);
}
