using Microsoft.AspNetCore.Http;
using XTI_App.Api;

namespace XTI_WebApp.Api;

internal sealed class DefaultIncludedLink : IIncludedLink
{
    private readonly CurrentUserAccess currentUserAccess;
    private readonly XtiBasePath xtiBasePath;
    private readonly LinkModel link;

    public DefaultIncludedLink(CurrentUserAccess currentUserAccess, XtiBasePath xtiBasePath, LinkModel link)
    {
        this.currentUserAccess = currentUserAccess;
        this.xtiBasePath = xtiBasePath;
        this.link = link;
    }

    public async Task<bool> IsIncluded()
    {
        bool isIncluded;
        if (link.IsXtiPath())
        {
            var xtiPath = link.GetXtiPath(xtiBasePath);
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
