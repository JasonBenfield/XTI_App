namespace XTI_App.Abstractions;

public interface IModifierCategory
{
    EntityID ID { get; }
    ModifierCategoryName Name();
    Task<IModifier> ModifierOrDefault(ModifierKey modKey);
}