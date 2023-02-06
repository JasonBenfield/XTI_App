namespace XTI_App.Abstractions;

public sealed record AppModel
(
    int ID, 
    AppKey AppKey, 
    AppVersionName VersionName,
    ModifierKey PublicKey
)
{
    public AppModel()
        :this(0, AppKey.Unknown, AppVersionName.Unknown, ModifierKey.Default)
    {
    }

    public bool IsUnknown() => AppKey.IsUnknown();
}