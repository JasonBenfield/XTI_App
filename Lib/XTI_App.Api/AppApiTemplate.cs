using XTI_App.Abstractions;

namespace XTI_App.Api;

public enum ApiCodeGenerators
{
    Dotnet = 0,
    TypeScript = 1,
    Other = 99
}

public sealed class AppApiTemplate
{
    private List<Func<ValueTemplate, ApiCodeGenerators, bool>> isExcludedFunctions = new List<Func<ValueTemplate, ApiCodeGenerators, bool>>();

    public AppApiTemplate(AppApi api)
    {
        AppKey = api.AppKey;
        GroupTemplates = api.Groups().Select(g => g.Template());
        RoleNames = api.RoleNames();
    }

    public AppKey AppKey { get; }
    public string Name { get => AppKey.Name.DisplayText.Replace(" ", ""); }
    public IEnumerable<AppRoleName> RoleNames { get; }
    public IEnumerable<AppApiGroupTemplate> GroupTemplates { get; }

    public void ExcludeValueTemplates(Func<ValueTemplate, ApiCodeGenerators, bool> isExcluded)
    {
        isExcludedFunctions.Add(isExcluded);
    }

    public IEnumerable<FormValueTemplate> FormTemplates(ApiCodeGenerators codeGenerator) =>
        excluding
        (
            GroupTemplates
                .SelectMany(g => g.FormTemplates())
                .Distinct(),
            codeGenerator
        );

    public IEnumerable<ObjectValueTemplate> ObjectTemplates(ApiCodeGenerators codeGenerator) =>
        excluding
        (
            GroupTemplates
                .SelectMany(g => g.ObjectTemplates())
                .Distinct(),
            codeGenerator
        );

    public IEnumerable<NumericValueTemplate> NumericValueTemplates(ApiCodeGenerators codeGenerator) =>
        excluding
        (
            GroupTemplates
                .SelectMany(g => g.NumericValueTemplates())
                .Distinct(),
            codeGenerator
        );

    public IEnumerable<EnumValueTemplate> EnumValueTemplates(ApiCodeGenerators codeGenerator) =>
        excluding
        (
            GroupTemplates
                .SelectMany(g => g.EnumValueTemplates())
                .Distinct(),
            codeGenerator
        );

    private T[] excluding<T>(IEnumerable<T> valueTemplates, ApiCodeGenerators codeGenerator)
        where T : ValueTemplate
    {
        var excluding = valueTemplates.Where(templ => isExcluded(templ, codeGenerator)).ToArray();
        return valueTemplates.Except(excluding).ToArray();
    }

    private bool isExcluded(ValueTemplate templ, ApiCodeGenerators codeGenerator)
    {
        bool isExcluded = false;
        foreach (var isExcludedFunc in isExcludedFunctions)
        {
            if (isExcludedFunc(templ, codeGenerator))
            {
                isExcluded = true;
                break;
            }
        }
        return isExcluded;
    }

    public AppApiTemplateModel ToModel()
        => new AppApiTemplateModel
        {
            AppKey = AppKey,
            GroupTemplates = GroupTemplates.Select(g => g.ToModel()).ToArray()
        };

    public bool IsAuthenticator() => Name.Equals("Hub", StringComparison.OrdinalIgnoreCase);
}