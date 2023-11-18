using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class DefaultIncludedLinkFactory : IIncludedLinkFactory
{
    private readonly CurrentUserAccess currentUserAccess;
    private readonly IXtiPathAccessor xtiPathAccessor;

    public DefaultIncludedLinkFactory(CurrentUserAccess currentUserAccess, IXtiPathAccessor xtiPathAccessor)
    {
        this.currentUserAccess = currentUserAccess;
        this.xtiPathAccessor = xtiPathAccessor;
    }

    public IIncludedLink Create(string menuName, LinkModel link) =>
        new DefaultIncludedLink(currentUserAccess, xtiPathAccessor, link);
}
