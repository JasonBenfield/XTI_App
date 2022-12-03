using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public interface ITransformedLinkFactory
{
    ITransformedLink Create(LinkModel link);
}
