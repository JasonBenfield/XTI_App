namespace XTI_App.Abstractions;

public sealed record AppApiGroupTemplateModel
(
    ResourceGroupName Name, 
    ModifierCategoryName ModCategory, 
    bool IsAnonymousAllowed, 
    AppRoleName[] Roles, 
    AppApiActionTemplateModel[] ActionTemplates
)
{
    public AppApiGroupTemplateModel()
        :this(ResourceGroupName.Unknown, ModifierCategoryName.Default, false, new AppRoleName[0], new AppApiActionTemplateModel[0])
    {
    }

    public AppRoleName[] RecursiveRoles()  => 
        Roles
            .Union(ActionTemplates.SelectMany(a => a.Roles))
            .Distinct()
            .ToArray();
}