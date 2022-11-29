namespace XTI_App.Abstractions;

public sealed class XtiPath : IEquatable<XtiPath>, IEquatable<string>
{
    public static XtiPath Parse(string str)
    {
        var parts = str.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var names = new List<string>(parts.Concat(Enumerable.Repeat("", parts.Length <= 5 ? 5 - parts.Length : 0)));
        var isOData = names[2] == "odata";
        var modifier = names[4];
        if (isOData && modifier == "$query")
        {
            modifier = "";
        }
        return new XtiPath
        (
            names[0],
            AppVersionKey.Parse(names[1]),
            new ResourceGroupName(isOData ? names[3] : names[2]),
            new ResourceName(isOData ? "Get" : names[3]),
            ModifierKey.FromValue(modifier)
        );
    }

    private readonly string value;
    private readonly int hashCode;

    public XtiPath(AppKey appKey)
        : this
        (
            appKey,
            AppVersionKey.Current,
            new ResourceGroupName(""),
            new ResourceName(""),
            ModifierKey.Default
        )
    {
    }

    public XtiPath(string appName, string version, string group, string action, ModifierKey modifier)
        : this
        (
             appName,
             string.IsNullOrWhiteSpace(version) ? AppVersionKey.Current : AppVersionKey.Parse(version),
             new ResourceGroupName(group),
             new ResourceName(action),
             modifier
        )
    {
    }

    public XtiPath
    (
        AppKey appKey,
        AppVersionKey version,
        ResourceGroupName group,
        ResourceName action,
        ModifierKey modifier
    )
        : this
        (
             appKey.Type.Equals(AppType.Values.WebService)
                  ? $"{appKey.Name.DisplayText}Service"
                  : appKey.Name.DisplayText,
             version,
             group,
             action,
             modifier
        )
    {
    }

    private XtiPath(string app, AppVersionKey version, ResourceGroupName group, ResourceName action, ModifierKey modifier)
    {
        if (string.IsNullOrWhiteSpace(app) && (!string.IsNullOrWhiteSpace(group.Value) || !string.IsNullOrWhiteSpace(action.Value))) { throw new ArgumentException($"{nameof(app)} is required"); }
        if (string.IsNullOrWhiteSpace(group.Value) && !string.IsNullOrWhiteSpace(action.Value)) { throw new ArgumentException($"{nameof(group)} is required when there is an action"); }
        App = app;
        Version = version;
        Group = group;
        Action = action;
        Modifier = modifier;
        value = $"/{App}/{Version.Value}/{Group.Value}/{Action.Value}/{Modifier.Value}";
        hashCode = value.GetHashCode();
    }

    public string App { get; }
    public AppVersionKey Version { get; }
    public ResourceGroupName Group { get; }
    public ResourceName Action { get; }
    public ModifierKey Modifier { get; }

    public bool IsCurrentVersion() => AppVersionKey.Current.Equals(Version);

    public void EnsureAppResource()
    {
        if (!string.IsNullOrWhiteSpace(Group.Value))
        {
            throw new ArgumentException($"{Format()} is not the name of an app");
        }
    }

    public void EnsureGroupResource()
    {
        if (string.IsNullOrWhiteSpace(Group.Value) || !string.IsNullOrWhiteSpace(Action.Value))
        {
            throw new ArgumentException($"{Format()} is not the name of a group");
        }
    }

    public void EnsureActionResource()
    {
        if (string.IsNullOrWhiteSpace(Action.Value))
        {
            throw new ArgumentException($"{Format()} is not the name of an action");
        }
    }

    public XtiPath WithNewGroup(XtiPath path)
    {
        var newPath = WithNewGroup(path.Group);
        if (!string.IsNullOrWhiteSpace(path.Action.Value))
        {
            newPath = newPath.WithAction(path.Action);
        }
        return newPath.WithModifier(path.Modifier);
    }

    public XtiPath WithNewGroup(string groupName)
        => WithNewGroup(new ResourceGroupName(groupName));

    public XtiPath WithNewGroup(ResourceGroupName groupName)
        => new XtiPath(App, Version, groupName, new ResourceName(""), Modifier);

    public XtiPath WithGroup(string groupName) => WithGroup(new ResourceGroupName(groupName));

    public XtiPath WithGroup(ResourceGroupName groupName)
    {
        if (!string.IsNullOrWhiteSpace(Group)) { throw new ArgumentException("Cannot create group for a group"); }
        return new XtiPath(App, Version, groupName, new ResourceName(""), Modifier);
    }

    public XtiPath WithAction(string actionName) => WithAction(new ResourceName(actionName));

    public XtiPath WithAction(ResourceName action)
    {
        if (!string.IsNullOrWhiteSpace(Action)) { throw new ArgumentException("Cannot create action for an action"); }
        return new XtiPath(App, Version, Group, action, Modifier);
    }

    public XtiPath WithModifier(ModifierKey modKey)
    {
        EnsureActionResource();
        return new XtiPath(App, Version, Group, Action, modKey);
    }

    public XtiPath WithVersion(AppVersionKey versionKey)
    {
        return new XtiPath(App, versionKey, Group, Action, Modifier);
    }

    public string Format()
    {
        var parts = new[]
        {
            App,
            Version.DisplayText,
            Group.DisplayText,
            Action.DisplayText,
            Modifier.DisplayText
        }
        .TakeWhile(str => !string.IsNullOrWhiteSpace(str));
        var joined = string.Join("/", parts.Select(part => part.Replace(" ", "")));
        return $"/{joined}";
    }

    public string RootPath()
    {
        var parts = new[]
        {
            Group.DisplayText,
            Action.DisplayText,
            Modifier.DisplayText
        }
        .TakeWhile(str => !string.IsNullOrWhiteSpace(str));
        var joined = string.Join("/", parts.Select(part => part.Replace(" ", "")));
        return $"~/{joined}";
    }

    public string Value() => Format().ToLower();

    public override bool Equals(object? obj)
    {
        if (obj is string str)
        {
            return Equals(str);
        }
        return Equals(obj as XtiPath);
    }

    public bool Equals(string? other) => Equals(Parse(other ?? ""));

    public bool Equals(XtiPath? other) => value == other?.value;

    public override int GetHashCode() => hashCode;

    public override string ToString()
    {
        var str = string.IsNullOrWhiteSpace(App) ? "Empty" : Format();
        return $"{nameof(XtiPath)} {str}";
    }

}