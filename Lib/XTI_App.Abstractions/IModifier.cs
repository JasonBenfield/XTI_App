namespace XTI_App.Abstractions;

public interface IModifier
{
    int ID { get; }
    ModifierKey ModKey();
}