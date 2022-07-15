namespace XTI_App.Abstractions;

public sealed record AppModel
(
    int ID, 
    AppKey AppKey, 
    AppVersionName VersionName,
    string Title,
    ModifierKey PublicKey
)
{
    public AppModel()
        :this(0, AppKey.Unknown, AppVersionName.Unknown, "", ModifierKey.Default)
    {
    }
}