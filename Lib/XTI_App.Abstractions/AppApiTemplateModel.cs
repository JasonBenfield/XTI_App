namespace XTI_App.Abstractions;

public sealed record AppApiTemplateModel(AppKey AppKey, string SerializedDefaultOptions, AppApiGroupTemplateModel[] GroupTemplates)
{
    public AppApiTemplateModel()
        : this(AppKey.Unknown, "", [])
    {
    }

    public AppRoleName[] RecursiveRoles() => 
        GroupTemplates.SelectMany(g => g.RecursiveRoles()).Distinct().ToArray();
}