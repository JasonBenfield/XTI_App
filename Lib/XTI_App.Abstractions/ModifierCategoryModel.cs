namespace XTI_App.Abstractions;

public sealed record ModifierCategoryModel
(
    int ID,
    ModifierCategoryName Name
)
{
    public ModifierCategoryModel()
        :this(0,ModifierCategoryName.Default)
    {
    }

    public bool IsDefault() => Name.Equals(ModifierCategoryName.Default);
}