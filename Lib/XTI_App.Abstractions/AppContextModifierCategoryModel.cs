namespace XTI_App.Abstractions;

public sealed record AppContextModifierCategoryModel
(
    ModifierCategoryModel ModifierCategory,
    ModifierModel[] Modifiers
)
{
    public AppContextModifierCategoryModel()
        :this(new ModifierCategoryModel(), new ModifierModel[0])
    {
    }
}