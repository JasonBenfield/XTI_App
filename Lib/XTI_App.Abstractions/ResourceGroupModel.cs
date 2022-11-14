namespace XTI_App.Abstractions;

public sealed record ResourceGroupModel
(
    int ID,
    int ModCategoryID,
    ResourceGroupName Name,
    bool IsAnonymousAllowed
)
{
    public ResourceGroupModel()
        : this(0, 0, ResourceGroupName.Unknown, false)
    {
    }

    public bool IsUnknown() => Name.Equals(ResourceGroupName.Unknown);
}