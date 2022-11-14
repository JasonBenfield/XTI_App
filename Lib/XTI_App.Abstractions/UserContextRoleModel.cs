namespace XTI_App.Abstractions;

public sealed record UserContextRoleModel(int ModifierCategoryID, ModifierKey ModifierKey, AppRoleModel[] Roles)
{
    public UserContextRoleModel()
        :this(0, ModifierKey.Default, new AppRoleModel[0])
    {
    }

    public bool IsExactMatch(int modCategoryID, ModifierKey modKey) =>
        ModifierCategoryID == modCategoryID && ModifierKey.Equals(modKey);

    public bool IsDefaultModifier() => ModifierKey.Equals(ModifierKey.Default);
}
