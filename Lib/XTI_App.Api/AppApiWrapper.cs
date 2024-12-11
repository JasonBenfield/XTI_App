using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Api;

public class AppApiWrapper : IAppApi
{
    protected readonly AppApi source;

    public AppApiWrapper(AppApi source)
    {
        this.source = source;
        Path = source.Path;
        AppKey = source.AppKey;
        Access = source.Access;
    }

    public XtiPath Path { get; }
    public AppKey AppKey { get; }
    public ResourceAccess Access { get; }
    public IAppApiGroup Group(string groupName) => source.Group(groupName);
    public IAppApiGroup[] Groups() => source.Groups();

    public bool HasAction(XtiPath xtiPath) => source.HasAction(xtiPath);

    public IAppApiAction GetAction(XtiPath xtiPath) => source.GetAction(xtiPath);

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
                    return ns.StartsWith("XTI_App.Abstractions") ||
                        ns.StartsWith("XTI_WebApp.Abstractions") ||
                        ns.StartsWith("XTI_Core");
                }
                var name = valueTemplate.DataType.Name ?? "";
                return name.Equals("EmptyRequest") ||
                    name.Equals("EmptyActionResult") ||
                    name.Equals("LogoutRequest") ||
                    name.Equals("ResourcePath") ||
                    name.Equals("ResourcePathAccess");
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