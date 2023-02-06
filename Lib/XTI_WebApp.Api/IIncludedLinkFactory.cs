using XTI_App.Abstractions;

namespace XTI_WebApp.Api;

public interface IIncludedLinkFactory
{
    IIncludedLink Create(string menuName, LinkModel link);
}
