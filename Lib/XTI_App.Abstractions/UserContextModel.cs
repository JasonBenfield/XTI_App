namespace XTI_App.Abstractions;

public sealed record UserContextModel(AppUserModel User, UserContextRoleModel[] ModifiedRoles)
{
    public UserContextModel()
        : this(new AppUserModel(), new UserContextRoleModel[0])
    {
    }

    public AppRoleModel[] GetRoles(ModifierModel modifier)
    {
        var modifiedRoles = ModifiedRoles.FirstOrDefault(r => r.Modifier.ID == modifier.ID);
        if (modifiedRoles == null)
        {
            modifiedRoles = ModifiedRoles.FirstOrDefault(r => r.IsDefaultModifier());
        }
        return modifiedRoles?.Roles ?? new AppRoleModel[0];
    }
}