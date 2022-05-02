using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeUserContext : ISourceUserContext
{
    private readonly FakeAppContext appContext;
    private readonly FakeCurrentUserName currentUserName;
    private readonly List<FakeAppUser> users = new();

    public FakeUserContext(FakeAppContext appContext, FakeCurrentUserName currentUserName)
    {
        this.appContext = appContext;
        AddUser(AppUserName.Anon);
        this.currentUserName = currentUserName;
    }

    public Task<AppUserName> CurrentUserName() => Task.FromResult(GetCurrentUserName());

    public AppUserName GetCurrentUserName() => currentUserName.GetUserName();

    Task<IAppUser> IUserContext.User() => Task.FromResult<IAppUser>(User());

    public FakeAppUser User() => User(currentUserName.GetUserName());

    Task<IAppUser> IUserContext.User(AppUserName userName) =>
        Task.FromResult<IAppUser>(User(userName));

    public FakeAppUser User(AppUserName userName) =>
        users.First(u => u.UserName().Equals(userName));

    public void SetCurrentUser(AppUserName userName) => currentUserName.SetUserName(userName);

    public FakeAppUser AddUser(IAppUser appUser)
    {
        var user = users.FirstOrDefault(u => u.ID.Equals(appUser.ID));
        if (user == null)
        {
            user = new FakeAppUser(appContext, appUser.ID, appUser.UserName());
            users.Add(user);
        }
        return user;
    }

    public FakeAppUser AddUser(AppUserName userName)
    {
        var user = users.FirstOrDefault(u => u.UserName().Equals(userName));
        if (user == null)
        {
            var id = getUniqueID();
            user = new FakeAppUser(appContext, id, userName);
            users.Add(user);
        }
        return user;
    }

    private int getUniqueID()
    {
        var id = FakeAppUser.NextID();
        while (users.Any(u => u.ID.Equals(id)))
        {
            id = FakeAppUser.NextID();
        }
        return id;
    }
}