namespace XTI_App.Api;

public sealed class AppApiGroupTemplateModel
{
    public string Name { get; set; } = "";
    public string ModCategory { get; set; } = "";
    public bool IsAnonymousAllowed { get; set; }
    public string[] Roles { get; set; } = new string[0];
    public AppApiActionTemplateModel[] ActionTemplates { get; set; } = new AppApiActionTemplateModel[0];

    public string[] RecursiveRoles()
        => Roles
            .Union(ActionTemplates.SelectMany(a => a.Roles))
            .Distinct()
            .ToArray();
}