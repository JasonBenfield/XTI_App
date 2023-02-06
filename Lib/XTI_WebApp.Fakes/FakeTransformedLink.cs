using XTI_App.Abstractions;
using XTI_WebApp.Api;

namespace XTI_WebApp.Fakes;

internal sealed class FakeTransformedLink : ITransformedLink
{
    private readonly LinkModel link;

    public FakeTransformedLink(LinkModel link)
    {
        this.link = link;
    }

    public Task<LinkModel> Value() => Task.FromResult(link);
}
