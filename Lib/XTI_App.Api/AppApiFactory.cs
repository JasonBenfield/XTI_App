namespace XTI_App.Api;

public abstract class AppApiFactory
{
    public IAppApi Create(IAppApiUser user) => _Create(user);
    public IAppApi CreateForSuperUser() => _Create(new AppApiSuperUser());
    public AppApiTemplate CreateTemplate() => CreateForSuperUser().Template();

    protected abstract IAppApi _Create(IAppApiUser user);
}