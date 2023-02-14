using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeUserContext : ISourceUserContext
{
    private readonly FakeAppContext appContext;
    private readonly FakeCurrentUserName currentUserName;
    private readonly List<UserContextModel> userContexts = new();

    private static int userID = 1001;

    public FakeUserContext(FakeAppContext appContext, FakeCurrentUserName currentUserName)
    {
        this.appContext = appContext;
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

    public UserContextModel GetUser(AppUserName userName) =>
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
            var id = GetUniqueID();
            user = new UserContextModel
            (
                new AppUserModel(id, userName, new PersonName(userName.DisplayText), "", DateTimeOffset.MaxValue),
                new UserContextRoleModel[0]
            );
            userContexts.Add(user);
        }
        return user;
    }

    public void DeactivateUser()
    {
        var user = GetUser(GetCurrentUserName());
        Update
        (
            user,
            u => u with
            {
                User = u.User with
                {
                    TimeDeactivated = DateTimeOffset.Now
                }
            }
        );
    }

    public void AddRolesToUser(params AppRoleName[] roles) =>
        AddRolesToUser(ModifierCategoryName.Default, ModifierKey.Default, roles);

    public void AddRolesToUser(ModifierCategoryName categoryName, ModifierKey modifierKey, params AppRoleName[] roles)
    {
        var user = GetUser(GetCurrentUserName());
        var modCategory = appContext.GetCurrentApp().ModifierCategory(categoryName);
        var modifier = modCategory.Modifier(modifierKey);
        var modifiedRole = user.ModifiedRoles
            .Where(mr => mr.Modifier.ModKey.Equals(modifierKey))
            .FirstOrDefault()
            ?? new UserContextRoleModel(modCategory.ModifierCategory, modifier, new AppRoleModel[0]);
        modifiedRole = modifiedRole with
        {
            Roles = modifiedRole.Roles
                .Union
                (
                    roles.Select(r => appContext.GetCurrentApp().Role(r)).ToArray()
                )
                .Distinct()
                .ToArray()
        };
        Update
        (
            user,
            u => u with
            {
                ModifiedRoles = u.ModifiedRoles
                    .Where(mr => !mr.Modifier.ModKey.Equals(modifierKey))
                    .Union(new[] { modifiedRole })
                    .ToArray()
            }
        );
    }

    public void SetUserRoles(params AppRoleName[] roles) =>
        SetUserRoles(ModifierCategoryName.Default, ModifierKey.Default, roles);

    public void SetUserRoles(ModifierCategoryName categoryName, ModifierKey modifierKey, params AppRoleName[] roles)
    {
        var user = GetUser(GetCurrentUserName());
        var modCategory = appContext.GetModifierCategory
        (
            appContext.GetCurrentApp(),
            categoryName
        );
        var modifier = modCategory.Modifier(modifierKey);
        var modifiedRole = user.ModifiedRoles
            .Where(mr => mr.Modifier.ModKey.Equals(modifierKey))
            .FirstOrDefault()
            ?? new UserContextRoleModel(modCategory.ModifierCategory, modifier, new AppRoleModel[0]);
        modifiedRole = modifiedRole with
        {
            Roles = roles.Select(r => appContext.GetCurrentApp().Role(r)).ToArray()
        };
        Update
        (
            user,
            u => u with
            {
                ModifiedRoles = u.ModifiedRoles
                    .Where(mr => !mr.Modifier.ModKey.Equals(modifierKey))
                    .Union(new[] { modifiedRole })
                    .ToArray()
            }
        );
    }

    private int GetUniqueID()
    {
        while (userContexts.Any(u => u.User.ID.Equals(userID)))
        {
            userID++;
        }
        return userID;
    }
}