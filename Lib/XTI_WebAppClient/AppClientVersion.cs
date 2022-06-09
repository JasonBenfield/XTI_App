namespace XTI_WebAppClient;

public sealed class AppClientVersion
{
    public static AppClientVersion Default() => new AppClientVersion("");

    public static AppClientVersion Current() => new AppClientVersion("Current");

    public static AppClientVersion Version(string versionKey) => new AppClientVersion(versionKey);

    private AppClientVersion(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public bool IsBlank() => string.IsNullOrWhiteSpace(Value);
}