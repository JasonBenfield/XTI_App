namespace XTI_App.Abstractions;

public sealed record AppApiActionTemplateModel(ResourceName Name, bool IsAnonymousAllowed, AppRoleName[] Roles, ResourceResultType ResultType)
{
    public AppApiActionTemplateModel()
        : this(ResourceName.Unknown, false, new AppRoleName[0], ResourceResultType.Values.GetDefault())
    {
    }
}