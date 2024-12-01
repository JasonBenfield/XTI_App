using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Api;

public class AppApiWrapper : IAppApi
{
    protected readonly AppApi source;

    public AppApiWrapper(AppApi source)
    {
        this.source = source;
    }

    public XtiPath Path { get => source.Path; }
    public AppKey AppKey { get => source.AppKey; }
    public ResourceAccess Access { get => source.Access; }
    public IAppApiGroup Group(string groupName) => source.Group(groupName);
    public IAppApiGroup[] Groups() => source.Groups();

    public AppApiTemplate Template()
    {
        var template = source.Template();
        template.ExcludeValueTemplates
        (
            (valueTemplate, codeGen) =>
            {
                if (codeGen == ApiCodeGenerators.Dotnet)
                {
                    var ns = valueTemplate.DataType.Namespace ?? "";
                    return ns.StartsWith("XTI_App.Abstractions") || ns.StartsWith("XTI_Core");
                }
                return false;
            }
        );
        ConfigureTemplate(template);
        return template;
    }

    internal AppRoleName[] RoleNames() => source.RoleNames();

    protected virtual void ConfigureTemplate(AppApiTemplate template) { }

    public ThrottledLogPath[] ThrottledLogPaths(XtiBasePath xtiBasePath) =>
        Groups().SelectMany(g => g.ThrottledLogPaths(xtiBasePath)).ToArray();

    public ScheduledAppAgendaItemOptions[] ActionSchedules() =>
        Groups().SelectMany(g => g.ActionSchedules()).ToArray();
}