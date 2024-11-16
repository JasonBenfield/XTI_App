namespace XTI_App.Abstractions;

public sealed record AppContextModel
(
    AppModel App,
    XtiVersionModel Version,
    AppRoleModel[] Roles,
    ModifierCategoryModel[] ModCategories,
    AppContextResourceGroupModel[] ResourceGroups,
    ModifierModel DefaultModifier
)
{
    public AppContextModel()
        : this
        (
            new AppModel(),
            new XtiVersionModel(),
            [],
            [],
            [],
            new()
        )
    {
    }

    public AppContextResourceModel Resource(ResourceGroupName groupName, ResourceName resourceName)
    {
        var resourceGroup = ResourceGroup(groupName);
        var resource = resourceGroup.Resources.FirstOrDefault(r => r.Resource.Name.Equals(resourceName));
        return resource ??
            throw new ArgumentException($"Resource '{groupName.DisplayText}'/'{resourceName.DisplayText}' not found.");
    }

    public AppContextResourceGroupModel ResourceGroup(ResourceGroupName groupName)
    {
        var resourceGroup = ResourceGroups
            .FirstOrDefault(rg => rg.ResourceGroup.Name.Equals(groupName));
        return resourceGroup ??
            throw new ArgumentException($"Resource Group '{groupName.DisplayText}' not found");
    }

    public ModifierCategoryModel DefaultModCategory() =>
        ModCategory(ModifierCategoryName.Default);

    public ModifierCategoryModel ModCategory(ModifierCategoryName categoryName) =>
        ModCategories.FirstOrDefault(c => c.Name.Equals(categoryName))
        ?? throw new ArgumentException($"Modifier Category '{categoryName.DisplayText}' not found.");

    public ModifierCategoryModel ModCategory(ResourceGroupModel group) =>
        ModCategory(group.Name);

    public ModifierCategoryModel ModCategory(ResourceGroupName name)
    {
        var resourceGroup = ResourceGroups
            .First(rg => rg.ResourceGroup.Name.Equals(name));
        var modCategoryID = resourceGroup.ResourceGroup.ModCategoryID;
        return ModCategories.FirstOrDefault(mc => mc.ID == modCategoryID)
            ?? throw new ArgumentException($"Modifier Category {modCategoryID} for Resource Group '{name.DisplayText}' not found.");
    }

    public AppRoleModel Role(AppRoleName name) =>
        Roles.FirstOrDefault(r => r.Name.Equals(name))
            ?? throw new ArgumentException($"Role '{name.DisplayText}' not found.");
}