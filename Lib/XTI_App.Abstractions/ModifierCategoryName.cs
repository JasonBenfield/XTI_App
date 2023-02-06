using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class ModifierCategoryName : TextKeyValue, IEquatable<ModifierCategoryName>
{
    public static readonly ModifierCategoryName Default = new("Default");

    public ModifierCategoryName(string value) : base(value)
    {
    }

    public bool Equals(ModifierCategoryName? other) => _Equals(other);
}