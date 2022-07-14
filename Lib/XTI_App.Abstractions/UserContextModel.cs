namespace XTI_App.Abstractions;

public sealed record UserContextModel(AppUserModel User, UserContextRoleModel[] ModifiedRoles)
{
    public UserContextModel()
        : this(new AppUserModel(), new UserContextRoleModel[0])
    {
    }

    public AppRoleModel[] GetRoles(ModifierKey modKey)
    {
        var role = ModifiedRoles.FirstOrDefault(r => r.ModifierKey.Equals(modKey));
        if (role == null && !modKey.Equals(ModifierKey.Default))
        {
            role = ModifiedRoles.FirstOrDefault(r => r.ModifierKey.Equals(ModifierKey.Default));
        }
        return role?.Roles ?? new AppRoleModel[0];
    }
}