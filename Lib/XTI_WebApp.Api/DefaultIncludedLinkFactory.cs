using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class DefaultIncludedLinkFactory : IIncludedLinkFactory
{
    private readonly CurrentUserAccess currentUserAccess;
    private readonly XtiBasePath xtiBasePath;

    public DefaultIncludedLinkFactory(CurrentUserAccess currentUserAccess, XtiBasePath xtiBasePath)
    {
        this.currentUserAccess = currentUserAccess;
        this.xtiBasePath = xtiBasePath;
    }

    public IIncludedLink Create(string menuName, LinkModel link) =>
        new DefaultIncludedLink(currentUserAccess, xtiBasePath, link);
}
