namespace XTI_App.Abstractions;

public sealed record XtiVersionModel
(
    int ID,
    AppVersionName VersionName,
    AppVersionKey VersionKey,
    AppVersionNumber VersionNumber,
    AppVersionType VersionType,
    AppVersionStatus Status,
    DateTimeOffset TimeAdded
)
{
    public XtiVersionModel()
        :this
        (
             0,
             AppVersionName.None,
             AppVersionKey.None,
             new AppVersionNumber(),
             AppVersionType.Values.GetDefault(),
             AppVersionStatus.Values.GetDefault(),
             DateTimeOffset.MaxValue
        )
    {
    }

    public AppVersionNumber NextPatch() => VersionNumber.NextPatch();
}