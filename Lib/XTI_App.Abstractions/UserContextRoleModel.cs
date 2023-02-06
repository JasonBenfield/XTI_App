namespace XTI_App.Abstractions;

public sealed record UserContextRoleModel(ModifierCategoryModel ModifierCategory, ModifierModel Modifier, AppRoleModel[] Roles)
{
    public UserContextRoleModel()
        :this(new ModifierCategoryModel(), new ModifierModel(), new AppRoleModel[0])
    {
    }

    public bool IsDefaultModifier() => 
        ModifierCategory.Name.Equals(ModifierCategoryName.Default) && 
        Modifier.ModKey.Equals(ModifierKey.Default);
}
