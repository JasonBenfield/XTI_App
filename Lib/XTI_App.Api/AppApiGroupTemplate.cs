using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiGroupTemplate
{
    public AppApiGroupTemplate(string name, ModifierCategoryName modCategory, ResourceAccess access, IEnumerable<AppApiActionTemplate> actionTemplates)
    {
        Name = name.Replace(" ", "");
        ModCategory = modCategory;
        Access = access;
        ActionTemplates = actionTemplates;
    }

    public string Name { get; }
    public ModifierCategoryName ModCategory { get; }
    public bool HasModifier { get => !ModCategory.Equals(ModifierCategoryName.Default); }
    public ResourceAccess Access { get; }
    public IEnumerable<AppApiActionTemplate> ActionTemplates { get; }

    public IEnumerable<FormValueTemplate> FormTemplates() =>
        ActionTemplates
            .SelectMany(a => a.FormTemplates())
            .Distinct();

    public IEnumerable<ObjectValueTemplate> ObjectTemplates() =>
        ActionTemplates
            .SelectMany(a => a.ObjectTemplates())
            .Distinct();

    public IEnumerable<QueryableValueTemplate> QueryableTemplates() =>
        ActionTemplates
            .SelectMany(a => a.QueryableTemplates())
            .Distinct();

    public IEnumerable<NumericValueTemplate> NumericValueTemplates() =>
        ActionTemplates
            .SelectMany(a => a.NumericValueTemplates())
            .Distinct();

    public IEnumerable<EnumValueTemplate> EnumValueTemplates() =>
        ActionTemplates
            .SelectMany(a => a.EnumValueTemplates())
            .Distinct();

    public bool IsUser() => Name.Equals("User", StringComparison.OrdinalIgnoreCase);

    public bool IsUserCache() => Name.Equals("UserCache", StringComparison.OrdinalIgnoreCase);

    public AppApiGroupTemplateModel ToModel() =>
        new
        (
            Name: new ResourceGroupName(Name),
            ModCategory: ModCategory,
            IsAnonymousAllowed: Access.IsAnonymousAllowed,
            Roles: Access.Allowed,
            ActionTemplates: ActionTemplates.Select(a => a.ToModel()).ToArray()
        );
}