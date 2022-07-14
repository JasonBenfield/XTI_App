namespace XTI_App.Abstractions;

public sealed record AppContextModel
(
    AppModel App,
    ModifierKey ModKey,
    XtiVersionModel Version,
    AppRoleModel[] Roles,
    AppContextResourceGroupModel[] ResourceGroups
)
{
    public AppContextModel()
        : this
        (
            new AppModel(), 
            ModifierKey.Default, 
            new XtiVersionModel(), 
            new AppRoleModel[0], 
            new AppContextResourceGroupModel[0]
        )
    {
    }

    public AppContextModifierCategoryModel ModCategory(ModifierCategoryName name) =>
        ResourceGroups.Select(rg => rg.ModifierCategory).First(mc => mc.Name.Equals(name));

    public AppRoleModel Role(AppRoleName name) => Roles.First(r => r.Name.Equals(name));
}