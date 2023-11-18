using Microsoft.AspNetCore.Http;
using XTI_App.Api;

namespace XTI_WebApp.Api;

internal sealed class DefaultIncludedLink : IIncludedLink
{
    private readonly CurrentUserAccess currentUserAccess;
    private readonly IXtiPathAccessor xtiPathAccessor;
    private readonly LinkModel link;

    public DefaultIncludedLink(CurrentUserAccess currentUserAccess, IXtiPathAccessor xtiPathAccessor, LinkModel link)
    {
        this.currentUserAccess = currentUserAccess;
        this.xtiPathAccessor = xtiPathAccessor;
        this.link = link;
    }

    public async Task<bool> IsIncluded()
    {
        var basePath = xtiPathAccessor.Value();
        bool isIncluded;
        if (link.IsXtiPath())
        {
            var xtiPath = link.GetXtiPath(basePath);
            if (xtiPath.Group.Equals(new ResourceGroupName("User")))
            {
                var isAnon = await currentUserAccess.IsAnon();
                isIncluded = !isAnon;
            }
            else
            {
                var accessResult = await currentUserAccess.HasAccess(xtiPath);
                isIncluded = accessResult.HasAccess;
            }
        }
        else
        {
            var isAnon = await currentUserAccess.IsAnon();
            isIncluded = !link.IsAuthenticationRequired || !isAnon;
        }
        return isIncluded;
    }
}
