using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeUserContext : ISourceUserContext
{
    private readonly FakeAppContext appContext;
    private AppUserName currentUserName = AppUserName.Anon;
    private readonly List<FakeAppUser> users = new List<FakeAppUser>();

    public FakeUserContext(FakeAppContext appContext)
    {
        this.appContext = appContext;
        AddUser(AppUserName.Anon);
        SetCurrentUser(AppUserName.Anon);
    }

    public Task<AppUserName> CurrentUserName() => Task.FromResult(currentUserName);

    Task<IAppUser> IUserContext.User() => Task.FromResult<IAppUser>(User());

    Task<IAppUser> ISourceUserContext.User(AppUserName userName) => 
        Task.FromResult<IAppUser>(User(userName));

    public FakeAppUser User() => User(currentUserName);

    public FakeAppUser User(AppUserName userName) =>
        users.First(u => u.UserName().Equals(userName));

    public void SetCurrentUser(AppUserName userName)
    {
        currentUserName = userName;
    }

    public FakeAppUser AddUser(AppUserName userName)
    {
        var user = new FakeAppUser(appContext, FakeAppUser.NextID(), userName);
        users.Add(user);
        return user;
    }
}