namespace XTI_App.Abstractions;

public sealed record AppContextModel
(
    AppModel App,
    XtiVersionModel Version,
    AppRoleModel[] Roles,
    AppContextModifierCategoryModel[] ModifierCategories,
    AppContextResourceGroupModel[] ResourceGroups
)
{
    public AppContextModel()
        : this
        (
            new AppModel(), 
            new XtiVersionModel(), 
            new AppRoleModel[0], 
            new AppContextModifierCategoryModel[0],
            new AppContextResourceGroupModel[0]
        )
    {
    }

    public AppContextModifierCategoryModel ModCategory(ResourceGroupName name)
    {
        var resourceGroup = ResourceGroups
            .First(rg => rg.ResourceGroup.Name.Equals(name));
        var modCategoryID = resourceGroup.ResourceGroup.ModCategoryID;
        return ModifierCategories.First(mc => mc.ModifierCategory.ID == modCategoryID);
    }

    public AppRoleModel Role(AppRoleName name) => Roles.First(r => r.Name.Equals(name));
}