namespace XTI_App.Abstractions;

public sealed record AppVersionNumber(int Major, int Minor, int Patch)
{
    public Version ToVersion() => new Version(Major, Minor, Patch);

    public AppVersionNumber NextPatch() => Next(AppVersionType.Values.Patch);

    public AppVersionNumber Next(AppVersionType type)
    {
        AppVersionNumber nextVersion;
        if (type.Equals(AppVersionType.Values.Major))
        {
            nextVersion = new AppVersionNumber(Major + 1, 0, 0);
        }
        else if (type.Equals(AppVersionType.Values.Minor))
        {
            nextVersion = new AppVersionNumber(Major, Minor + 1, 0);
        }
        else
        {
            nextVersion = new AppVersionNumber(Major, Minor, Patch + 1);
        }
        return nextVersion;
    }

    public AppVersionNumber()
        : this(0, 0, 0)
    {
    }

    public string Format() => $"{Major}.{Minor}.{Patch}";

    public string FormatAsDev() => $"{Format()}-dev{DateTime.Now:yyMMddHHmmssfff}";
}