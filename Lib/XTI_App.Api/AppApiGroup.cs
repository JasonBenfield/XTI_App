using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Api;

public sealed class AppApiGroup 
{
    private readonly IServiceProvider sp;
    private readonly List<IAppApiActionBuilder> actionBuilders = new();
    private Dictionary<string, IAppApiAction>? actions;
    private readonly ResourceAccessBuilder accessBuilder;

    public AppApiGroup
    (
        IServiceProvider sp,
        XtiPath path,
        ModifierCategoryName modCategory,
        ResourceAccessBuilder appAccessBuilder,
        IAppApiUser user
    )
    {
        this.sp = sp;
        Path = path;
        ModCategory = modCategory;
        accessBuilder = new ResourceAccessBuilder(appAccessBuilder);
        User = user;
    }

    public IAppApiUser User { get; }
    internal ModifierCategoryName ModCategory { get; }

    public bool HasAction(string actionName) =>
        FetchActionDictionary().ContainsKey(FormatActionKey(actionName));

    public TAppApiAction Action<TAppApiAction>(string actionName)
        where TAppApiAction : IAppApiAction =>
        (TAppApiAction)Action(actionName);

    public IAppApiAction Action(string actionName)
    {
        var actionKey = FormatActionKey(actionName);
        var dict = FetchActionDictionary();
        IAppApiAction action;
        if (dict.ContainsKey(actionKey))
        {
            action = dict[FormatActionKey(actionName)];
        }
        else
        {
            throw new Exception($"Action '{actionName}' not found in group '{Path.Group.DisplayText}'");
        }
        return action;
    }

    public IAppApiAction[] Actions() => FetchActionDictionary().Values.ToArray();

    private Dictionary<string, IAppApiAction> FetchActionDictionary() =>
        actions ??= actionBuilders.ToDictionary(ab => FormatActionKey(ab.ActionName), ab => ab.Build());

    public XtiPath Path { get; }
    public string GroupName { get => Path.Group.DisplayText.Replace(" ", ""); }
    internal ResourceAccess Access { get => accessBuilder.Build(); }

    public AppApiGroup AllowAnonymousAccess()
    {
        accessBuilder.AllowAnonymous();
        return this;
    }

    public AppApiGroup ResetAccess()
    {
        accessBuilder.Reset();
        return this;
    }

    public AppApiGroup ResetAccess(ResourceAccess access)
    {
        accessBuilder.Reset(access);
        return this;
    }

    public AppApiGroup ResetAccessWithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.Reset(roleNames);
        return this;
    }

    public AppApiGroup WithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithAllowed(roleNames);
        return this;
    }

    public AppApiGroup WithoutAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithoutAllowed(roleNames);
        return this;
    }

    public ScheduledAppAgendaItemOptions[] ActionSchedules() =>
        Actions()
            .Select(a => a.Schedule)
            .Where(s => !s.IsDisabled)
            .ToArray();

    public AppApiGroupTemplate Template()
    {
        var actionTemplates = Actions().Select(a => a.Template());
        return new AppApiGroupTemplate(Path.Group.DisplayText, ModCategory, Access, actionTemplates);
    }

    public AppRoleName[] RoleNames()
    {
        var roleNames = new List<AppRoleName>();
        roleNames.AddRange(Access.Allowed);
        var actionRoleNames = Actions()
            .SelectMany(a => a.Access.Allowed)
            .Distinct();
        roleNames.AddRange(actionRoleNames);
        return roleNames.Distinct().ToArray();
    }

    public AppApiAction<TModel, TResult> AddAction<TModel, TResult>
    (
        string actionName,
        Func<AppAction<TModel, TResult>> createExecution,
        Func<AppActionValidation<TModel>>? createValidation = null,
        ResourceAccess? access = null,
        string friendlyName = ""
    )
    {
        var builder = AddAction<TModel, TResult>(actionName)
            .WithExecution(createExecution)
            .WithFriendlyName(friendlyName);
        if (access != null)
        {
            builder.ResetAccess(access);
        }
        if (createValidation != null)
        {
            builder.WithValidation(createValidation);
        }
        return builder.Build();
    }

    public AppApiActionBuilder<TModel, TResult> AddAction<TModel, TResult>() =>
        AddAction<TModel, TResult>("");

    public AppApiActionBuilder<TModel, TResult> AddAction<TModel, TResult>(string actionName)
    {
        var action = new AppApiActionBuilder<TModel, TResult>(sp, Path, User, accessBuilder);
        action.Named(actionName);
        actionBuilders.Add(action);
        return action;
    }

    public IAppApiActionBuilder AddAction
    (
        string actionName,
        Func<AddActionData, IAppApiActionBuilder> createAction
    )
    {
        var path = Path.WithAction(actionName);
        var action = createAction
        (
            new AddActionData
            (
                Services: sp,
                ActionPath: path,
                GroupAccessBuilder: accessBuilder,
                User: User
            )
        );
        actionBuilders.Add(action);
        return action;
    }

    private static string FormatActionKey(string actionName) =>
        actionName.ToLower().Replace(" ", "").Replace("_", "");

    public override string ToString() => $"{GetType().Name} {Path.Group}";

    public ThrottledLogPath[] ThrottledLogPaths(XtiBasePath xtiBasePath) =>
        Actions().Select(a => a.ThrottledLogPath(xtiBasePath)).ToArray();
}

public sealed record AddActionData(IServiceProvider Services, XtiPath ActionPath, ResourceAccessBuilder GroupAccessBuilder, IAppApiUser User);