namespace XTI_App.Abstractions;

public interface IModifierCategory
{
    int ID { get; }
    ModifierCategoryName Name();
    Task<IModifier> ModifierOrDefault(ModifierKey modKey);
}