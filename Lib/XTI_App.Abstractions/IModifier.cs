namespace XTI_App.Abstractions;

public interface IModifier
{
    EntityID ID { get; }
    ModifierKey ModKey();
}