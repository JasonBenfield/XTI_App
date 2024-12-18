using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApi
{
    private readonly IServiceProvider sp;
    private readonly IAppApiUser user;
    private readonly Dictionary<string, AppApiGroup> groups = new();
    private Dictionary<string, IAppApiGroup>? groupWrappers;
    private readonly ResourceAccessBuilder accessBuilder = new();

    public AppApi
    (
        IServiceProvider sp,
        AppKey appKey,
        IAppApiUser user,
        string serializedDefaultOptions = ""
    )
    {
        this.sp = sp;
        AppKey = appKey;
        Path = new XtiPath(appKey);
        this.user = user;
        accessBuilder = new ResourceAccessBuilder();
        accessBuilder.WithAllowed(AppRoleName.Admin);
        SerializedDefaultOptions = serializedDefaultOptions;
    }

    public string SerializedDefaultOptions { get; set; }

    public XtiPath Path { get; }

    public AppKey AppKey { get; }

    public ResourceAccess Access { get => accessBuilder.Build(); }

    public AppApi AllowAnonymousAccess()
    {
        accessBuilder.AllowAnonymous();
        return this;
    }

    public AppApi ResetAccess()
    {
        accessBuilder.Reset();
        return this;
    }

    public AppApi ResetAccessWithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.Reset(roleNames);
        return this;
    }

    public AppApi WithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithAllowed(roleNames);
        return this;
    }

    public AppApi WithoutAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithoutAllowed(roleNames);
        return this;
    }

    public AppApiGroup AddGroup(string name) =>
        _AddGroup(name, ModifierCategoryName.Default, null);

    public AppApiGroup AddGroup(string name, ModifierCategoryName modCategory) =>
        _AddGroup(name, modCategory, null);

    public AppApiGroup AddGroup(string name, ResourceAccess access) =>
        _AddGroup(name, ModifierCategoryName.Default, access);

    public AppApiGroup AddGroup(string name, ModifierCategoryName modCategory, ResourceAccess access) =>
        _AddGroup(name, modCategory, access);

    private AppApiGroup _AddGroup(string name, ModifierCategoryName modCategory, ResourceAccess? access)
    {
        var group = new AppApiGroup(sp, Path.WithGroup(name), modCategory, accessBuilder, user);
        if(access != null)
        {
            group.ResetAccess(access);
        }
        groups.Add(FormatGroupKey(group.GroupName), group);
        return group;
    }

    public bool HasAction(XtiPath xtiPath)
    {
        bool hasAction;
        var groupKey = FormatGroupKey(xtiPath.Group.DisplayText);
        if (groups.ContainsKey(groupKey))
        {
            var group = groups[groupKey];
            hasAction = group.HasAction(xtiPath.Action.DisplayText);
        }
        else
        {
            hasAction = false;
        }
        return hasAction;
    }

    public IAppApiAction GetAction(XtiPath xtiPath)
    {
        IAppApiAction action;
        var groupKey = FormatGroupKey(xtiPath.Group.DisplayText);
        if (groups.ContainsKey(groupKey))
        {
            var group = groups[groupKey];
            action = group.Action(xtiPath.Action.DisplayText);
        }
        else
        {
            throw new Exception($"Action not found for path {xtiPath.Value()}");
        }
        return action;
    }

    public IAppApiGroup[] Groups() => FetchGroupWrappers().Values.ToArray();

    public IAppApiGroup Group(string groupName) => FetchGroupWrappers()[FormatGroupKey(groupName)];

    private Dictionary<string, IAppApiGroup> FetchGroupWrappers() =>
        groupWrappers ??= 
            groups.ToDictionary
            (
                kvp => kvp.Key, 
                kvp => (IAppApiGroup)new AppApiGroupWrapper(kvp.Value)
            );

    private static string FormatGroupKey(string groupName) =>
        groupName.ToLower().Replace(" ", "").Replace("_", "");

    public AppApiTemplate Template()
    {
        var template = new AppApiTemplate(this, SerializedDefaultOptions);
        configureTemplate(template);
        return template;
    }

    private Action<AppApiTemplate> configureTemplate = (t) => { };

    public void ConfigureTemplate(Action<AppApiTemplate> configureTemplate)
    {
        this.configureTemplate = configureTemplate;
    }

    internal AppRoleName[] RoleNames()
    {
        var roleNames = new List<AppRoleName>();
        roleNames.AddRange(Access.Allowed);
        var groupRoleNames = groups
            .Values
            .SelectMany(g => g.RoleNames())
            .Distinct();
        roleNames.AddRange(groupRoleNames);
        return roleNames.Distinct().ToArray();
    }

    public override string ToString() => $"{GetType().Name} {Path.App}";
}