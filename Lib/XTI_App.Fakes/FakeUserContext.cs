using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeUserContext : ISourceUserContext
{
    private readonly FakeAppContext appContext;
    private readonly FakeCurrentUserName currentUserName;
    private readonly List<AppUserModel> users = new();
    private readonly Dictionary<string, AppRoleModel[]> userRoles = new();

    private static int userID = 1001;

    public FakeUserContext(FakeAppContext appContext, FakeCurrentUserName currentUserName)
    {
        this.appContext = appContext;
        AddUser(AppUserName.Anon);
        this.currentUserName = currentUserName;
    }

    public AppUserName GetCurrentUserName() => currentUserName.GetUserName();

    public Task<AppUserModel> User() => User(GetCurrentUserName());

    public Task<AppUserModel> User(AppUserName userName)
    {
        return Task.FromResult(GetUser(userName));
    }

    public AppUserModel GetUser() => GetUser(GetCurrentUserName());

    public AppUserModel GetUser(AppUserName userName) =>
        users.First(u => u.UserName.Equals(userName));

    public void SetCurrentUser(AppUserName userName) => currentUserName.SetUserName(userName);

    public AppUserModel Update(AppUserModel original, Func<AppUserModel, AppUserModel> update)
    {
        users.Remove(original);
        var updated = update(original);
        users.Add(updated);
        return updated;
    }

    public AppUserModel AddUser(AppUserModel user)
    {
        users.RemoveAll(u => u.ID == user.ID || u.UserName.Equals(user.UserName));
        users.Add(user);
        return user;
    }

    public AppUserModel AddUser(AppUserName userName)
    {
        var user = users.FirstOrDefault(u => u.UserName.Equals(userName));
        if (user == null)
        {
            var id = GetUniqueID();
            user = new AppUserModel
            (
                id,
                userName,
                new PersonName(userName.DisplayText),
                "",
                DateTimeOffset.MaxValue
            );
            users.Add(user);
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
                TimeDeactivated = DateTimeOffset.Now
            }
        );
    }

    public void AddRolesToUser(params AppRoleName[] roles) =>
        AddRolesToUser(ModifierCategoryName.Default, ModifierKey.Default, roles);

    public void AddRolesToUser(ModifierCategoryName categoryName, ModifierKey modifierKey, params AppRoleName[] roleNames)
    {
        var user = GetUser(GetCurrentUserName());
        var modCategory = appContext.GetModCategory(categoryName);
        var modifier = appContext.GetModifier(modCategory, modifierKey);
        var userRoleKey = GetUserRoleKey(user, modifier);
        var roles = appContext.GetRoles(roleNames);
        if (userRoles.ContainsKey(userRoleKey))
        {
            userRoles[userRoleKey] = userRoles[userRoleKey]
                .Union(roles)
                .Distinct()
                .ToArray();
        }
        else
        {
            userRoles.Add(userRoleKey, roles);
        }
    }

    public void SetUserRoles(params AppRoleName[] roles) =>
        SetUserRoles(ModifierCategoryName.Default, ModifierKey.Default, roles);

    public void SetUserRoles(ModifierCategoryModel modCategory, ModifierKey modifierKey, params AppRoleName[] roleNames) =>
        SetUserRoles(modCategory.Name, modifierKey, roleNames);

    public void SetUserRoles(ModifierCategoryName categoryName, ModifierKey modifierKey, params AppRoleName[] roleNames)
    {
        var user = GetUser(GetCurrentUserName());
        var modCategory = appContext.GetModCategory(categoryName);
        var modifier = appContext.GetModifier(modCategory, modifierKey);
        var userRoleKey = GetUserRoleKey(user, modifier);
        var roles = appContext.GetRoles(roleNames);
        if (userRoles.ContainsKey(userRoleKey))
        {
            userRoles[userRoleKey] = roles;
        }
        else
        {
            userRoles.Add(userRoleKey, roles);
        }
    }

    private int GetUniqueID()
    {
        while (users.Any(u => u.ID.Equals(userID)))
        {
            userID++;
        }
        return userID;
    }

    public Task<AppRoleModel[]> UserRoles(AppUserModel user, ModifierModel modifier)
    {
        var roles = GetUserRoles(user, modifier);
        return Task.FromResult(roles);
    }

    private AppRoleModel[] GetUserRoles(AppUserModel user, ModifierModel modifier)
    {
        if (!userRoles.TryGetValue(GetUserRoleKey(user, modifier), out var roles))
        {
            var defaultModifier = appContext.GetDefaultModifier();
            if(!userRoles.TryGetValue(GetUserRoleKey(user, defaultModifier), out roles))
            {
                roles = new AppRoleModel[0];
            }
        }
        return roles;
    }

    private static string GetUserRoleKey(AppUserModel user, ModifierModel modifier) =>
        $"{user.UserName.Value}_{modifier.ID}";
}