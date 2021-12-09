using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApi
{
    public AppApi
    (
        AppKey appKey,
        IAppApiUser user,
        ResourceAccess access
    )
    {
        AppKey = appKey;
        Path = new XtiPath(appKey.Name.DisplayText);
        this.user = user;
        Access = access ?? ResourceAccess.AllowAuthenticated();
    }

    private readonly IAppApiUser user;
    private readonly Dictionary<string, AppApiGroup> groups = new Dictionary<string, AppApiGroup>();

    public XtiPath Path { get; }

    public AppKey AppKey { get; }

    public ResourceAccess Access { get; }

    public AppApiGroup AddGroup(string name)
        => AddGroup(name, ModifierCategoryName.Default, Access);

    public AppApiGroup AddGroup(string name, ModifierCategoryName modCategory)
        => AddGroup(name, modCategory, Access);

    public AppApiGroup AddGroup(string name, ResourceAccess access)
        => AddGroup(name, ModifierCategoryName.Default, access);

    public AppApiGroup AddGroup(string name, ModifierCategoryName modCategory, ResourceAccess access)
    {
        var group = new AppApiGroup(Path.WithGroup(name), modCategory, access, user);
        groups.Add(groupKey(group.GroupName), group);
        return group;
    }

    public IEnumerable<AppApiGroup> Groups() => groups.Values.ToArray();

    public AppApiGroup Group(string groupName) => groups[groupKey(groupName)];

    private static string groupKey(string groupName)
        => groupName.ToLower().Replace(" ", "").Replace("_", "");

    public AppApiTemplate Template() => new AppApiTemplate(this);

    internal IEnumerable<AppRoleName> RoleNames()
    {
        var roleNames = new List<AppRoleName>();
        roleNames.AddRange(Access.Allowed);
        var groupRoleNames = groups
            .Values
            .SelectMany(g => g.RoleNames())
            .Distinct();
        roleNames.AddRange(groupRoleNames);
        return roleNames.Distinct();
    }

    public override string ToString() => $"{GetType().Name} {Path.App}";
}