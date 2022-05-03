namespace XTI_App.Api;

public sealed class AppApiSuperUser : IAppApiUser
{
    public Task<bool> HasAccess(ResourceAccess resourceAccess) => Task.FromResult(true);

    public Task EnsureUserHasAccess(ResourceAccess resourceAccess) => Task.CompletedTask;
}