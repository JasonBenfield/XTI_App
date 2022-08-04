namespace XTI_App.Abstractions;

public sealed record AppContextResourceGroupModel
(
    ResourceGroupModel ResourceGroup,
    AppContextResourceModel[] Resources,
    AppRoleModel[] AllowedRoles
)
{
    public AppContextResourceGroupModel()
        :this
        (
            new ResourceGroupModel(), 
            new AppContextResourceModel[0],
            new AppRoleModel[0]
        )
    {
    }
}