namespace XTI_App.Abstractions;

public sealed record UserContextModel(AppUserModel User, UserContextRoleModel[] ModifiedRoles)
{
    public UserContextModel()
        : this(new AppUserModel(), new UserContextRoleModel[0])
    {
    }

    public AppRoleModel[] GetRoles(int modCategoryID, ModifierKey modKey)
    {
        var role = ModifiedRoles.FirstOrDefault(r => r.ModifierCategoryID == modCategoryID && r.ModifierKey.Equals(modKey));
        if (role == null)
        {
            role = ModifiedRoles.FirstOrDefault(r => r.ModifierKey.Equals(ModifierKey.Default));
        }
        return role?.Roles ?? new AppRoleModel[0];
    }
}