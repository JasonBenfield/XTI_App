using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using XTI_Core;

namespace XTI_App.Abstractions;

[TypeConverter(typeof(AppVersionKeyTypeConverter))]
[JsonConverter(typeof(AppVersionKeyJsonConverter))]
public sealed partial class AppVersionKey : TextValue, IEquatable<AppVersionKey>
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
            if (!KeyRegex().IsMatch(str))
            {
                throw new ArgumentException($"'{str}' is not a valid version key");
            }
            key = new AppVersionKey(str);
        }
        return key;
    }

    public AppVersionKey() : this(0)
    {
    }

    public AppVersionKey(int versionID) : this($"V{versionID:00000}")
    {
    }

    private AppVersionKey(string key) : base(key)
    {
    }

    public bool IsNone() => Equals(None);

    public bool IsCurrent() => Equals(Current);

    public bool Equals(AppVersionKey? other)
    {
        bool isEqual;
        if(other == null)
        {
            isEqual = false;
        }
        else if(other.Equals(Current.Value) && Equals(Current.Value))
        {
            isEqual = true;
        }
        else if(other.Equals(None.Value) && Equals(None.Value))
        {
            isEqual = true;
        }
        else if(other.Equals(None.Value) || Equals(None.Value) || other.Equals(Current.Value) || Equals(Current.Value))
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
}