using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiTemplateModel
{
    public AppKey AppKey { get; set; } = AppKey.Unknown;
    public AppApiGroupTemplateModel[] GroupTemplates { get; set; } = new AppApiGroupTemplateModel[0];

    public string[] RecursiveRoles()
        => GroupTemplates.SelectMany(g => g.RecursiveRoles()).Distinct().ToArray();
}