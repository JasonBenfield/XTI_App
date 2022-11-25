using System.Reflection.Metadata.Ecma335;

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

    public AppContextModifierCategoryModel ModCategory(ResourceGroupName name)
    {
        var resourceGroup = ResourceGroups
            .First(rg => rg.ResourceGroup.Name.Equals(name));
        var modCategoryID = resourceGroup.ResourceGroup.ModCategoryID;
        return ModifierCategories.FirstOrDefault(mc => mc.ModifierCategory.ID == modCategoryID)
            ?? throw new ArgumentException($"Modifier Category {modCategoryID} for Resource Group '{name.DisplayText}' not found.");
    }

    public AppRoleModel Role(AppRoleName name) => 
        Roles.FirstOrDefault(r => r.Name.Equals(name))
            ?? throw new ArgumentException($"Role '{name.DisplayText}' not found.");
}