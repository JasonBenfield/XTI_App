namespace XTI_App.Abstractions;

public sealed record AppContextResourceGroupModel
(
    int ID,
    ResourceGroupName Name,
    bool IsAnonymousAllowed,
    AppContextModifierCategoryModel ModifierCategory,
    AppContextResourceModel[] Resources
)
{
    public AppContextResourceGroupModel()
        :this
        (
            0,
            ResourceGroupName.Unknown, 
            false, 
            new AppContextModifierCategoryModel(), 
            new AppContextResourceModel[0]
        )
    {
    }
}