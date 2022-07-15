using System.Text.RegularExpressions;
using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class ModifierKey : TextKeyValue, IEquatable<ModifierKey>
{
    private static readonly Regex replaceRegex = new("\\s+");


    public static readonly ModifierKey Default = new ModifierKey("");

    public static ModifierKey FromValue(string value) =>
        string.IsNullOrWhiteSpace(value) ? Default : new ModifierKey(value);

    public ModifierKey(string value)
        : base(replaceRegex.Replace(value, ""))
    {
    }

    public bool Equals(ModifierKey? other) => _Equals(other);
}