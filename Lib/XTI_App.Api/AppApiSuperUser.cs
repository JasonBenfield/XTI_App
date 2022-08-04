using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiSuperUser : IAppApiUser
{
    public Task<bool> HasAccess(XtiPath path) => Task.FromResult(true);

    public Task EnsureUserHasAccess(XtiPath path) => Task.CompletedTask;
}