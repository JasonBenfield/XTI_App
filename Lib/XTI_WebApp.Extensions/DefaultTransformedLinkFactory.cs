using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class DefaultTransformedLinkFactory : ITransformedLinkFactory
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IUserContext userContext;

    public DefaultTransformedLinkFactory(IHttpContextAccessor httpContextAccessor, IUserContext userContext)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.userContext = userContext;
    }

    public ITransformedLink Create(LinkModel link) => 
        new DefaultTransformedLink(httpContextAccessor, userContext, link);
}
