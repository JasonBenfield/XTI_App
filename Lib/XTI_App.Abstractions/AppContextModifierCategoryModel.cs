namespace XTI_App.Abstractions;

public sealed record AppContextModifierCategoryModel
(
    int ID,
    ModifierCategoryName Name,
    ModifierModel[] Modifiers
)
{
    public AppContextModifierCategoryModel()
        :this(0, ModifierCategoryName.Default, new ModifierModel[0])
    {
    }
}