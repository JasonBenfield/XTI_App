using XTI_App.Abstractions;

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
    public IEnumerable<IAppApiGroup> Groups() => source.Groups();
    public AppApiTemplate Template()
    {
        var template = source.Template();
        template.ExcludeValueTemplates
        (
            (templ, codeGen) =>
            {
                if(codeGen == ApiCodeGenerators.Dotnet)
                {
                    var ns = templ.DataType.Namespace ?? "";
                    return ns.StartsWith("XTI_App.Abstractions") || ns.StartsWith("XTI_Core");
                }
                return false;
            }
        );
        ConfigureTemplate(template);
        return template;
    }

    protected virtual void ConfigureTemplate(AppApiTemplate template) { }
}