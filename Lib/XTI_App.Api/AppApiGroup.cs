using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App.Api;

public sealed class AppApiGroup : IAppApiGroup
{
    private readonly ModifierCategoryName modCategory;
    private readonly IAppApiUser user;
    private readonly Dictionary<string, IAppApiAction> actions = new();

    public AppApiGroup
    (
        XtiPath path,
        ModifierCategoryName modCategory,
        ResourceAccess access,
        IAppApiUser user
    )
    {
        Path = path;
        this.modCategory = modCategory ?? ModifierCategoryName.Default;
        Access = access ?? ResourceAccess.AllowAuthenticated();
        this.user = user;
    }

    public IAppApiUser User { get => user; }

    public TAppApiAction Action<TAppApiAction>(string actionName) where TAppApiAction : IAppApiAction =>
        (TAppApiAction)actions[actionKey(actionName)];

    public IEnumerable<IAppApiAction> Actions() => actions.Values.ToArray();

    public XtiPath Path { get; }
    public string GroupName { get => Path.Group.DisplayText.Replace(" ", ""); }
    public ResourceAccess Access { get; }

    public Task<bool> HasAccess() => user.HasAccess(Path);

    public AppApiGroupTemplate Template()
    {
        var actionTemplates = Actions().Select(a => a.Template());
        return new AppApiGroupTemplate(Path.Group.DisplayText, modCategory, Access, actionTemplates);
    }

    internal IEnumerable<AppRoleName> RoleNames()
    {
        var roleNames = new List<AppRoleName>();
        roleNames.AddRange(Access.Allowed);
        var actionRoleNames = Actions()
            .SelectMany(a => a.Access.Allowed)
            .Distinct();
        roleNames.AddRange(actionRoleNames);
        return roleNames.Distinct();
    }

    public AppApiAction<TModel, TResult> AddAction<TModel, TResult>
    (
        string actionName,
        Func<AppAction<TModel, TResult>> createExecution,
        Func<AppActionValidation<TModel>>? createValidation = null,
        ResourceAccess? access = null,
        string friendlyName = ""
    ) =>
        (AppApiAction<TModel, TResult>)AddAction
        (
            actionName,
            friendlyName,
            (addData) =>
                new AppApiAction<TModel, TResult>
                (
                    addData.ActionPath,
                    access ?? addData.GroupAccess,
                    addData.User,
                    createValidation ?? defaultCreateValidation<TModel>(),
                    createExecution,
                    addData.FriendlyName
                )
        );

    public static Func<AppActionValidation<TModel>> defaultCreateValidation<TModel>() =>
        () => new AppActionValidationNotRequired<TModel>();

    public IAppApiAction AddAction
    (
        string actionName,
        string friendlyName,
        Func<AddActionData, IAppApiAction> createAction
    )
    {
        var path = Path.WithAction(actionName);
        friendlyName = string.IsNullOrWhiteSpace(friendlyName)
            ? string.Join(" ", new CamelCasedWord(path.Action.DisplayText).Words())
            : friendlyName;
        var action = createAction
        (
            new AddActionData
            (
                ActionPath: path, 
                GroupAccess: Access, 
                User: User, 
                FriendlyName: friendlyName
            )
        );
        actions.Add(actionKey(action.ActionName), action);
        return action;
    }

    private AppApiAction<TModel, TResult> AddAction<TModel, TResult>(AppApiAction<TModel, TResult> action)
    {
        actions.Add(actionKey(action.ActionName), action);
        return action;
    }

    private static string actionKey(string actionName) =>
        actionName.ToLower().Replace(" ", "").Replace("_", "");

    public override string ToString() => $"{GetType().Name} {Path.Group}";
}

public sealed record AddActionData(XtiPath ActionPath, ResourceAccess GroupAccess, IAppApiUser User, string FriendlyName);