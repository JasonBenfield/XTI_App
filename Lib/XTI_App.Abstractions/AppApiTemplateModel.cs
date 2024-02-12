namespace XTI_App.Abstractions;

public sealed record AppApiTemplateModel(AppKey AppKey, AppApiGroupTemplateModel[] GroupTemplates)
{
    public AppApiTemplateModel()
        : this(AppKey.Unknown, new AppApiGroupTemplateModel[0])
    {
    }

    public AppRoleName[] RecursiveRoles()
        => GroupTemplates.SelectMany(g => g.RecursiveRoles()).Distinct().ToArray();
}