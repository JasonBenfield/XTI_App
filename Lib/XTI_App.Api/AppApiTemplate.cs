using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiTemplate
{
    private List<Func<ValueTemplate, ApiCodeGenerators, bool>> isExcludedFunctions = new();

    public AppApiTemplate(AppApi api, string serializedDefaultOptions)
    {
        AppKey = api.AppKey;
        GroupTemplates = api.Groups().Select(g => g.Template()).ToArray();
        RoleNames = api.RoleNames();
        SerializedDefaultOptions = serializedDefaultOptions;
    }

    public AppKey AppKey { get; }
    public string Name { get => AppKey.Name.DisplayText.Replace(" ", ""); }
    public AppRoleName[] RoleNames { get; }
    public AppApiGroupTemplate[] GroupTemplates { get; }
    public string SerializedDefaultOptions { get; }

    public void ExcludeValueTemplates(Func<ValueTemplate, ApiCodeGenerators, bool> isExcluded)
    {
        isExcludedFunctions.Add(isExcluded);
    }

    public IEnumerable<FormValueTemplate> FormTemplates(ApiCodeGenerators codeGenerator) =>
        Excluding
        (
            GroupTemplates
                .SelectMany(g => g.FormTemplates())
                .Distinct(),
            codeGenerator
        );

    public IEnumerable<QueryableValueTemplate> QueryableTemplates(ApiCodeGenerators codeGenerator) =>
        Excluding
        (
            GroupTemplates
                .SelectMany(g => g.QueryableTemplates())
                .Distinct(),
            codeGenerator
        );

    public IEnumerable<ObjectValueTemplate> ObjectTemplates(ApiCodeGenerators codeGenerator) =>
        Excluding
        (
            GroupTemplates
                .SelectMany(g => g.ObjectTemplates())
                .Distinct(),
            codeGenerator
        );

    public IEnumerable<NumericValueTemplate> NumericValueTemplates(ApiCodeGenerators codeGenerator) =>
        Excluding
        (
            GroupTemplates
                .SelectMany(g => g.NumericValueTemplates())
                .Distinct(),
            codeGenerator
        );

    public IEnumerable<EnumValueTemplate> EnumValueTemplates(ApiCodeGenerators codeGenerator) =>
        Excluding
        (
            GroupTemplates
                .SelectMany(g => g.EnumValueTemplates())
                .Distinct(),
            codeGenerator
        );

    private T[] Excluding<T>(IEnumerable<T> valueTemplates, ApiCodeGenerators codeGenerator)
        where T : ValueTemplate
    {
        var excluding = valueTemplates.Where(templ => IsExcluded(templ, codeGenerator)).ToArray();
        return valueTemplates.Except(excluding).ToArray();
    }

    private bool IsExcluded(ValueTemplate templ, ApiCodeGenerators codeGenerator)
    {
        var isExcluded = false;
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

    public AppApiTemplateModel ToModel() =>
        new
        (
            AppKey: AppKey,
            SerializedDefaultOptions: SerializedDefaultOptions,
            GroupTemplates: GroupTemplates.Select(g => g.ToModel()).ToArray()
        );

    public bool IsAuthenticator() => Name.Equals("Hub", StringComparison.OrdinalIgnoreCase);
}