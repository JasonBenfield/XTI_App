using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using XTI_Core;

namespace XTI_App.Abstractions;

[TypeConverter(typeof(AppVersionKeyTypeConverter))]
[JsonConverter(typeof(AppVersionKeyJsonConverter))]
public sealed partial class AppVersionKey : TextValue, IEquatable<AppVersionKey>, IComparable<AppVersionKey>
{
    public static readonly AppVersionKey None = new();
    public static readonly AppVersionKey Current = new("Current");

    public static AppVersionKey Parse(string str)
    {
        AppVersionKey key;
        if (string.IsNullOrWhiteSpace(str))
        {
            key = None;
        }
        else if (str.Equals("Current", StringComparison.OrdinalIgnoreCase))
        {
            key = Current;
        }
        else
        {
            if (KeyRegex().IsMatch(str))
            {
                key = new AppVersionKey(str);
            }
            else
            {
                key = None;
            }
        }
        return key;
    }

    private readonly int sortValue;

    public AppVersionKey() : this(0)
    {
    }

    public AppVersionKey(int versionID) : this($"V{versionID:00000}")
    {
    }

    private AppVersionKey(string key) : base(key)
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            sortValue = int.MaxValue;
        }
        else if (Value.Equals("Current", StringComparison.OrdinalIgnoreCase))
        {
            sortValue = 0;
        }
        else
        {
            sortValue = int.Parse(key.Substring(1));
        }
    }

    public bool IsNone() => Equals(None);

    public bool IsCurrent() => Equals(Current);

    public bool Equals(AppVersionKey? other)
    {
        bool isEqual;
        if (other == null)
        {
            isEqual = false;
        }
        else if (other.Equals(Current.Value) && Equals(Current.Value))
        {
            isEqual = true;
        }
        else if (other.Equals(None.Value) && Equals(None.Value))
        {
            isEqual = true;
        }
        else if (other.Equals(None.Value) || Equals(None.Value) || other.Equals(Current.Value) || Equals(Current.Value))
        {
            isEqual = false;
        }
        else
        {
            isEqual =
                int.TryParse(other.Value.Substring(1), out var otherVersionID) &&
                int.TryParse(Value.Substring(1), out var versionID) &&
                otherVersionID == versionID;
        }
        return isEqual;
    }

    [GeneratedRegex("V?(\\d+)")]
    private static partial Regex KeyRegex();

    public int CompareTo(AppVersionKey? other) => sortValue.CompareTo(other?.sortValue ?? int.MaxValue);
}