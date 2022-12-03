using XTI_App.Api;
using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class GetMenuLinksAction : AppAction<string, LinkModel[]>
{
    private readonly AppMenuDefinitions appMenuDefinitions;
    private readonly IIncludedLinkFactory includedLinkFactory;
    private readonly ITransformedLinkFactory transformedLinkFactory;

    public GetMenuLinksAction(AppMenuDefinitions appMenuDefinitions, IIncludedLinkFactory includedLinkFactory, ITransformedLinkFactory transformedLinkFactory)
    {
        this.appMenuDefinitions = appMenuDefinitions;
        this.includedLinkFactory = includedLinkFactory;
        this.transformedLinkFactory = transformedLinkFactory;
    }

    public async Task<LinkModel[]> Execute(string menuName, CancellationToken stoppingToken)
    {
        var links = appMenuDefinitions.GetLinks(menuName);
        var includedLinks = new List<LinkModel>();
        foreach(var link in links)
        {
            var includedLink = includedLinkFactory.Create(menuName, link);
            var isIncluded = await includedLink.IsIncluded();
            if (isIncluded)
            {
                var transformedLink = await transformedLinkFactory.Create(link).Value();
                includedLinks.Add(transformedLink);
            }
        }
        return includedLinks.ToArray();
    }
}
