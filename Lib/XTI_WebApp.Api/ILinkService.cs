using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public interface ILinkService
{
    Task<LinkModel[]> GetLinks(string menuName);
}
