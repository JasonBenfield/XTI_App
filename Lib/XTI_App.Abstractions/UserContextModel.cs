namespace XTI_App.Abstractions;

public sealed record UserContextModel(AppUserModel User, UserContextRoleModel[] ModifiedRoles)
{
    public UserContextModel()
        : this(new AppUserModel(), new UserContextRoleModel[0])
    {
    }

    public AppRoleModel[] GetRoles(int modCategoryID, ModifierKey modKey)
    {
        var modifiedRoles = ModifiedRoles.FirstOrDefault(r => r.IsExactMatch(modCategoryID, modKey));
        if (modifiedRoles == null)
        {
            modifiedRoles = ModifiedRoles.FirstOrDefault(r => r.IsDefaultModifier());
        }
        return modifiedRoles?.Roles ?? new AppRoleModel[0];
    }
}