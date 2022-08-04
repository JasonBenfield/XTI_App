using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface IAppApiUser
{
    Task<bool> HasAccess(XtiPath path);
    Task EnsureUserHasAccess(XtiPath path);
}