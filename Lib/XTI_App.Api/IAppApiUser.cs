using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface IAppApiUser
{
    Task EnsureUserHasAccess(XtiPath path);
}