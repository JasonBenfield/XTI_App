using XTI_App.Api;
using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public interface IIncludedLinkFactory
{
    IIncludedLink Create(string menuName, LinkModel link);
}
