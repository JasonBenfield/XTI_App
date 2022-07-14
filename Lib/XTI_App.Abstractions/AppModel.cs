namespace XTI_App.Abstractions;

public sealed record AppModel
(
    int ID, 
    AppKey AppKey, 
    AppVersionName VersionName,
    string Title
)
{
    public AppModel()
        :this(0, AppKey.Unknown, AppVersionName.Unknown, "")
    {
    }
}