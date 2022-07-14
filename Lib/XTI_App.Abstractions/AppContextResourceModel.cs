namespace XTI_App.Abstractions;

public sealed record AppContextResourceModel
(
    int ID,
    ResourceName Name,
    bool IsAnonymousAllowed,
    ResourceResultType ResultType,
    AppRoleModel[] AllowedRoles
)
{
    public AppContextResourceModel()
        :this
        (
            0, 
            ResourceName.Unknown, 
            false, 
            ResourceResultType.Values.GetDefault(), 
            new AppRoleModel[0]
        )
    {
    }
}