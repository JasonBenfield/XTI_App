using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeAppUser : IAppUser
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static int NextID() => currentID.Next();

    private readonly FakeAppContext appContext;
    private readonly AppUserName userName;
    private readonly Dictionary<int, FakeAppRole[]> roles = new Dictionary<int, FakeAppRole[]>();

    public FakeAppUser(FakeAppContext appContext, int id, AppUserName userName)
    {
        this.appContext = appContext;
        ID = id;
        this.userName = userName;
    }

    public int ID { get; }

    public AppUserName UserName() => userName;

    public void RemoveRole(AppRoleName roleNameToRemove)
        => RemoveRoles(roleNameToRemove);

    public void RemoveRole(IModifier modifier, AppRoleName roleNameToRemove)
        => RemoveRoles(modifier, roleNameToRemove);

    public void RemoveRole(FakeAppRole roleToRemove)
        => RemoveRoles(roleToRemove);

    public void RemoveRole(IModifier modifier, FakeAppRole roleToRemove)
        => RemoveRoles(modifier, roleToRemove);

    public void RemoveRoles(params AppRoleName[] roleNamesToRemove)
    {
        var app = appContext.App();
        var defaultModifier = app.DefaultModifier();
        RemoveRoles(defaultModifier, roleNamesToRemove);
    }

    public void RemoveRoles(IModifier modifier, params AppRoleName[] roleNamesToRemove)
    {
        var app = appContext.App();
        var roles = new List<FakeAppRole>();
        foreach (var roleName in roleNamesToRemove)
        {
            var role = app.Role(roleName);
            roles.Add(role);
        }
        RemoveRoles(modifier, roles.ToArray());
    }

    public void RemoveRoles(params FakeAppRole[] rolesToRemove)
    {
        var app = appContext.App();
        var defaultModifier = app.DefaultModifier();
        RemoveRoles(defaultModifier, rolesToRemove);
    }

    public void RemoveRoles(IModifier modifier, params FakeAppRole[] rolesToRemove)
    {
        if (roles.ContainsKey(modifier.ID))
        {
            var originalRoles = roles[modifier.ID];
            roles[modifier.ID] = originalRoles.Except(rolesToRemove).Distinct().ToArray();
        }
    }

    public void AddRole(AppRoleName roleToAdd)
        => AddRoles(roleToAdd);

    public void AddRole(ModifierCategoryName categoryName, ModifierKey modKey, AppRoleName roleToAdd)
        => AddRoles(getModifier(categoryName, modKey), roleToAdd);

    public void AddRole(FakeModifier modifier, AppRoleName roleToAdd)
        => AddRoles(modifier, roleToAdd);

    public void AddRoles(params AppRoleName[] roleNamesToAdd)
    {
        var app = appContext.App();
        var roles = new List<FakeAppRole>();
        foreach (var roleName in roleNamesToAdd)
        {
            var role = app.Role(roleName);
            roles.Add(role);
        }
        AddRoles(roles.ToArray());
    }

    public void AddRoles(ModifierCategoryName categoryName, ModifierKey modKey, params AppRoleName[] roleNamesToAdd)
    {
        AddRoles(getModifier(categoryName, modKey), roleNamesToAdd);
    }

    private FakeModifier getModifier(ModifierCategoryName categoryName, ModifierKey modKey)
    {
        var app = appContext.App();
        var modCategory = app.ModCategory(categoryName);
        var modifier = modCategory.ModifierOrDefault(modKey);
        return modifier;
    }

    public void AddRoles(FakeModifier modifier, params AppRoleName[] roleNamesToAdd)
    {
        var app = appContext.App();
        var roles = new List<FakeAppRole>();
        foreach (var roleName in roleNamesToAdd)
        {
            var role = app.Role(roleName);
            roles.Add(role);
        }
        AddRoles(modifier, roles.ToArray());
    }

    public void AddRole(FakeAppRole roleToAdd) => AddRoles(roleToAdd);

    public void AddRole(FakeModifier modifier, FakeAppRole roleToAdd) => AddRoles(modifier, roleToAdd);

    public void AddRoles(params FakeAppRole[] rolesToAdd)
    {
        var app = appContext.App();
        var defaultModifier = app.DefaultModifier();
        AddRoles(defaultModifier, rolesToAdd);
    }

    public void AddRoles(FakeModifier modifier, params FakeAppRole[] rolesToAdd)
    {
        if (roles.ContainsKey(modifier.ID))
        {
            var originalRoles = roles[modifier.ID];
            roles[modifier.ID] = originalRoles.Union(rolesToAdd).Distinct().ToArray();
        }
        else
        {
            roles.Add(modifier.ID, rolesToAdd);
        }
    }

    Task<IAppRole[]> IAppUser.Roles(IModifier modifier) =>
        Task.FromResult<IAppRole[]>(Roles(modifier));

    public FakeAppRole[] Roles(ModifierCategoryName categoryName, ModifierKey modKey) =>
        Roles(getModifier(categoryName, modKey));

    public FakeAppRole[] Roles(IModifier modifier)
    {
        if (!roles.TryGetValue(modifier.ID, out var userRoles))
        {
            if (modifier.ModKey().Equals(ModifierKey.Default))
            {
                userRoles = new FakeAppRole[0];
            }
            else
            {
                var app = appContext.App();
                var defaultModCategory = app.ModCategory(ModifierCategoryName.Default);
                var defaultMod = defaultModCategory.ModifierOrDefault(ModifierKey.Default);
                userRoles = Roles(defaultMod);
            }
        }
        return userRoles;
    }

}