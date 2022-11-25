using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;

namespace XTI_WebApp.Fakes;

public sealed class FakeLinkService : ILinkService
{
    private readonly Dictionary<string, LinkModel[]> menus = new();

    public void AddMenu(string menuName, params LinkModel[] links) => menus.Add(menuName, links);

    public Task<LinkModel[]> GetLinks(string menuName) =>
        Task.FromResult(menus[menuName]);
}
