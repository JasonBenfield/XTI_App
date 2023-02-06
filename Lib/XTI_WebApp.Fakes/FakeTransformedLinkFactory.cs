using XTI_App.Abstractions;
using XTI_WebApp.Api;

namespace XTI_WebApp.Fakes;

public sealed class FakeTransformedLinkFactory : ITransformedLinkFactory
{
    public ITransformedLink Create(LinkModel link) => new FakeTransformedLink(link);
}
