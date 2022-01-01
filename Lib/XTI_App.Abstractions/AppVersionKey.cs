using System.ComponentModel;
using System.Text.RegularExpressions;
using XTI_Core;

namespace XTI_App.Abstractions;

[TypeConverter(typeof(AppVersionKeyTypeConverter))]
public sealed class AppVersionKey : TextValue, IEquatable<AppVersionKey>
{
    private static readonly Regex keyRegex = new Regex("V?(\\d+)");

    public static readonly AppVersionKey None = new AppVersionKey();
    public static readonly AppVersionKey Current = new AppVersionKey("Current");

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
            if (!keyRegex.IsMatch(str))
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

    public AppVersionKey(int versionID) : this($"V{versionID}")
    {
    }

    private AppVersionKey(string key) : base(key)
    {
    }

    public bool Equals(AppVersionKey? other) => _Equals(other);
}