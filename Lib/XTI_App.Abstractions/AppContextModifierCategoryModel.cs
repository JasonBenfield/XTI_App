namespace XTI_App.Abstractions;

public sealed record AppContextModifierCategoryModel
(
    ModifierCategoryModel ModifierCategory,
    ModifierModel[] Modifiers
)
{
    public AppContextModifierCategoryModel()
        : this(new ModifierCategoryModel(), new ModifierModel[0])
    {
    }

    public bool ModifierExists(ModifierKey modKey) => Modifiers.Any(m => m.ModKey.Equals(modKey));

    public ModifierModel Modifier(ModifierKey modKey) =>
        Modifiers.FirstOrDefault(m => m.ModKey.Equals(modKey))
        ?? throw new Exception($"Modifier with key '{modKey.DisplayText}' not found");
}