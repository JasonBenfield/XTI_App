namespace XTI_App.Abstractions;

public sealed record AppContextResourceModel
(
    ResourceModel Resource,
    AppRoleModel[] AllowedRoles
)
{
    public AppContextResourceModel()
        : this
        (
            new ResourceModel(),
            new AppRoleModel[0]
        )
    {
    }
}