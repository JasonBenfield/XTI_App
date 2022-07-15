namespace XTI_App.Abstractions;

public sealed record AppContextResourceGroupModel
(
    ResourceGroupModel ResourceGroup,
    AppContextResourceModel[] Resources
)
{
    public AppContextResourceGroupModel()
        :this
        (
            new ResourceGroupModel(), 
            new AppContextResourceModel[0]
        )
    {
    }
}