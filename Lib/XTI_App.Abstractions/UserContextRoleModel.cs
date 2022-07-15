namespace XTI_App.Abstractions;

public sealed record UserContextRoleModel(int ModifierCategoryID, ModifierKey ModifierKey, AppRoleModel[] Roles)
{
    public UserContextRoleModel()
        :this(0, ModifierKey.Default, new AppRoleModel[0])
    {
    }
}
