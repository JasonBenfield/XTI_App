namespace XTI_App.Abstractions;

public sealed record AppRoleModel
(
    int ID,
    AppRoleName Name
)
{
    public AppRoleModel()
        :this(0, AppRoleName.DenyAccess)
    {
    }
}