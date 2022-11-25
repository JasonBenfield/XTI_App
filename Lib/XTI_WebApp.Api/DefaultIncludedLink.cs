using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_WebApp.Abstractions;

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
            var accessResult = await currentUserAccess.HasAccess(xtiPath);
            isIncluded = accessResult.HasAccess;
        }
        else
        {
            isIncluded = true;
        }
        return isIncluded;
    }
}
