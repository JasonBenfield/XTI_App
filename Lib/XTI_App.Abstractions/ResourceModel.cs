namespace XTI_App.Abstractions;

public sealed record ResourceModel
(
    int ID,
    ResourceName Name,
    bool IsAnonymousAllowed,
    ResourceResultType ResultType
)
{
    public ResourceModel()
        :this(0, ResourceName.Unknown, false, ResourceResultType.Values.GetDefault())
    {
    }

    public bool IsUnknown() => Name.Equals(ResourceName.Unknown);

}