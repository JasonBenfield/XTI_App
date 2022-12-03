using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public interface ITransformedLink
{
    Task<LinkModel> Value();
}
