using System.Text.RegularExpressions;
using XTI_Core;

namespace XTI_App.Abstractions;

public sealed partial class ModifierKey : TextKeyValue, IEquatable<ModifierKey>
{
    public static readonly ModifierKey Default = new();

    public static ModifierKey FromValue(string value) =>
        string.IsNullOrWhiteSpace(value) ? Default : new(value);

    public ModifierKey()
        : this("")
    {
    }

    public ModifierKey(string value)
        : base(ReplaceRegex().Replace(value, "").ToLower(), ReplaceRegex().Replace(value, ""))
    {
    }

    public bool Equals(ModifierKey? other) => _Equals(other);

    [GeneratedRegex("\\s+")]
    private static partial Regex ReplaceRegex();
}