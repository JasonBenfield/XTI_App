namespace XTI_App.Abstractions;

public sealed record UserContextRoleModel(ModifierKey ModifierKey, AppRoleModel[] Roles)
{
    public UserContextRoleModel()
        :this(ModifierKey.Default, new AppRoleModel[0])
    {
    }
}
