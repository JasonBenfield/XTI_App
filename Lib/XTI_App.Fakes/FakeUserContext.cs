using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeUserContext : ISourceUserContext
{
    private readonly FakeCurrentUserName currentUserName;
    private readonly List<UserContextModel> userContexts = new();

    private static int userID = 1001;

    public FakeUserContext(FakeCurrentUserName currentUserName)
    {
        AddUser(AppUserName.Anon);
        this.currentUserName = currentUserName;
    }

    public AppUserName GetCurrentUserName() => currentUserName.GetUserName();

    public Task<UserContextModel> User() => User(GetCurrentUserName());

    public Task<UserContextModel> User(AppUserName userName)
    {
        return Task.FromResult(GetUser(userName));
    }

    public UserContextModel GetUser() => GetUser(GetCurrentUserName());

    public UserContextModel GetUser(AppUserName userName)=>
        userContexts.First(u => u.User.UserName.Equals(userName));

    public void SetCurrentUser(AppUserName userName) => currentUserName.SetUserName(userName);

    public UserContextModel Update(UserContextModel original, Func<UserContextModel, UserContextModel> update)
    {
        userContexts.Remove(original);
        var updated = update(original);
        userContexts.Add(updated);
        return updated;
    }

    public UserContextModel AddUser(UserContextModel userContext)
    {
        userContexts.RemoveAll(u => u.User.ID == userContext.User.ID);
        userContexts.Add(userContext);
        return userContext;
    }

    public UserContextModel AddUser(AppUserName userName)
    {
        var user = userContexts.FirstOrDefault(u => u.User.UserName.Equals(userName));
        if (user == null)
        {
            var id = getUniqueID();
            user = new UserContextModel
            (
                new AppUserModel(id, userName, new PersonName(userName.Value), ""),
                new UserContextRoleModel[0]
            );
            userContexts.Add(user);
        }
        return user;
    }

    private int getUniqueID()
    {
        while (userContexts.Any(u => u.User.ID.Equals(userID)))
        {
            userID++;
        }
        return userID;
    }
}