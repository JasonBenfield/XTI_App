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

    public FakeAppUser AddUser(IAppUser appUser)
    {
        var user = users.FirstOrDefault(u => u.ID.Equals(appUser.ID));
        if(user == null)
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

    private EntityID getUniqueID()
    {
        var id = FakeAppUser.NextID();
        while (users.Any(u => u.ID.Equals(id)))
        {
            id = FakeAppUser.NextID();
        }
        return id;
    }
}