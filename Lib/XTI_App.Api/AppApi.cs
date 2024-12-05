using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApi
{
    private readonly IServiceProvider sp;
    private readonly IAppApiUser user;
    private readonly Dictionary<string, AppApiGroup> groups = new();

    public AppApi
    (
        IServiceProvider sp,
        AppKey appKey,
        IAppApiUser user,
        ResourceAccess access,
        string serializedDefaultOptions
    )
    {
        this.sp = sp;
        AppKey = appKey;
        Path = new XtiPath(appKey);
        this.user = user;
        Access = access ?? ResourceAccess.AllowAuthenticated();
        SerializedDefaultOptions = serializedDefaultOptions;
    }

    internal string SerializedDefaultOptions { get; }

    public XtiPath Path { get; }

    public AppKey AppKey { get; }

    public ResourceAccess Access { get; }

    public AppApiGroup AddGroup(string name) =>
        AddGroup(name, ModifierCategoryName.Default, Access);

    public AppApiGroup AddGroup(string name, ModifierCategoryName modCategory) =>
        AddGroup(name, modCategory, Access);

    public AppApiGroup AddGroup(string name, ResourceAccess access) =>
        AddGroup(name, ModifierCategoryName.Default, access);

    public AppApiGroup AddGroup(string name, ModifierCategoryName modCategory, ResourceAccess access)
    {
        var group = new AppApiGroup(sp, Path.WithGroup(name), modCategory, access, user);
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

    public AppApiGroup[] Groups() => groups.Values.ToArray();

    public AppApiGroup Group(string groupName) => groups[FormatGroupKey(groupName)];

    private static string FormatGroupKey(string groupName) =>
        groupName.ToLower().Replace(" ", "").Replace("_", "");

    public AppApiTemplate Template() => new(this, SerializedDefaultOptions);

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