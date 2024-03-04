namespace XTI_App.Abstractions;

public sealed record ModifierModel
(
    int ID,
    int CategoryID,
    ModifierKey ModKey,
    string TargetKey,
    string DisplayText
)
{
    public ModifierModel()
        : this(0, 0, ModifierKey.Default, "", "")
    {
    }

    public bool IsFound() => ID > 0;

    public bool IsDefault() => ModKey.Equals(ModifierKey.Default);
}