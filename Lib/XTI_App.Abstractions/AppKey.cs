using System.ComponentModel;
using System.Text.Json.Serialization;

namespace XTI_App.Abstractions;

[TypeConverter(typeof(AppKeyTypeConverter))]
[JsonConverter(typeof(AppKeyJsonConverter))]
public sealed class AppKey : IEquatable<AppKey>
{
    public static readonly AppKey Unknown = new AppKey();

    public static AppKey WebApp(string name) => WebApp(new AppName(name));
    public static AppKey WebApp(AppName name) => new AppKey(name, AppType.Values.WebApp);
    public static AppKey WebPackage(string name) => WebPackage(new AppName(name));
    public static AppKey WebPackage(AppName name) => new AppKey(name, AppType.Values.WebPackage);
    public static AppKey ServiceApp(string name) => ServiceApp(new AppName(name));
    public static AppKey ServiceApp(AppName name) => new AppKey(name, AppType.Values.ServiceApp);
    public static AppKey ConsoleApp(string name) => ConsoleApp(new AppName(name));
    public static AppKey ConsoleApp(AppName name) => new AppKey(name, AppType.Values.ConsoleApp);
    public static AppKey Package(string name) => Package(new AppName(name));
    public static AppKey Package(AppName name) => new AppKey(name, AppType.Values.Package);

    public static AppKey Parse(string value)
    {
        var split = value.Split(new[] { '\\', '/', '|', ':', ';' });
        return new AppKey(split[0], AppType.Values.Value(split[1]));
    }

    private readonly string value;

    public AppKey()
        :this(AppName.Unknown, AppType.Values.NotFound)
    {
    }

    public AppKey(string name, AppType appType)
        : this(new AppName(name), appType)
    {
    }

    public AppKey(AppName name, AppType appType)
    {
        Name = name;
        Type = appType;
        value = $"{Name.Value}:{Type.DisplayText}";
    }

    public AppName Name { get; }
    public AppType Type { get; }

    public string Format() => $"{Name.DisplayText} {Type.DisplayText}";

    public string Serialize() => value;

    public override string ToString() => $"{nameof(AppKey)} {Format()}";

    public bool IsUnknown() => Equals(Unknown);

    public bool IsAppType(AppType otherAppType) => Type.Equals(otherAppType);

    public bool IsAnyAppType(params AppType[] otherAppTypes) => Type.EqualsAny(otherAppTypes);

    public bool Equals(AppKey? other) => value == other?.value;

    public override bool Equals(object? obj)
    {
        if (obj is AppKey appKey)
        {
            return Equals(appKey);
        }
        return base.Equals(obj);
    }

    public override int GetHashCode() => value.GetHashCode();
}