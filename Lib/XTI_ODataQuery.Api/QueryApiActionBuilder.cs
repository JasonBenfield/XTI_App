using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public sealed class QueryApiActionBuilder<TArgs, TEntity> : IAppApiActionBuilder
{
    private readonly IServiceProvider sp;
    private readonly XtiPath groupPath;
    private readonly IAppApiUser user;
    private string friendlyName = "";
    private readonly ActionThrottledLogPathBuilder<QueryApiActionBuilder<TArgs, TEntity>> throttledLogPathBuilder;
    private ResourceAccessBuilder accessBuilder;
    private Func<QueryAction<TArgs, TEntity>> createQuery;
    private QueryApiAction<TArgs, TEntity>? builtAction;

    internal QueryApiActionBuilder
    (
        IServiceProvider sp,
        XtiPath groupPath,
        IAppApiUser user,
        ResourceAccessBuilder groupAccessBuilder,
        string actionName
    )
    {
        this.sp = sp;
        this.groupPath = groupPath;
        this.user = user;
        accessBuilder = new ResourceAccessBuilder(groupAccessBuilder);
        throttledLogPathBuilder = new(this);
        ActionName = actionName;
        createQuery = () => new EmptyQueryAction<TArgs, TEntity>();
    }

    public string ActionName { get; }

    public QueryApiActionBuilder<TArgs, TEntity> WithFriendlyName(string friendlyName)
    {
        this.friendlyName = friendlyName;
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> AllowAnonymousAccess()
    {
        accessBuilder.AllowAnonymous();
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> ResetAccess()
    {
        accessBuilder.Reset();
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> ResetAccess(ResourceAccess access)
    {
        accessBuilder.Reset(access);
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> ResetAccessWithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.Reset(roleNames);
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> WithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithAllowed(roleNames);
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> WithoutAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithoutAllowed(roleNames);
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> WithQuery<TExecution>() where TExecution : QueryAction<TArgs, TEntity>
    {
        WithQuery(() => sp.GetRequiredService<TExecution>());
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> WithQuery(Func<QueryAction<TArgs, TEntity>> createQuery)
    {
        this.createQuery = createQuery;
        return this;
    }

    public ActionThrottledLogIntervalBuilder<QueryApiActionBuilder<TArgs, TEntity>> ThrottleRequestLogging() =>
        throttledLogPathBuilder.RequestIntervalBuilder;

    public ActionThrottledLogIntervalBuilder<QueryApiActionBuilder<TArgs, TEntity>> ThrottleExceptionLogging() =>
        throttledLogPathBuilder.ExceptionIntervalBuilder;

    public XtiPath ActionPath() => groupPath.WithAction(ActionName);

    public QueryApiAction<TArgs, TEntity> Build()
    {
        builtAction ??= BuildAction();
        return builtAction;
    }

    private QueryApiAction<TArgs, TEntity> BuildAction()
    {
        var actionPath = ActionPath();
        var scheduledBuilder = new ActionScheduleBuilder<QueryApiActionBuilder<TArgs, TEntity>>(this);
        return new QueryApiAction<TArgs, TEntity>
        (
            actionPath,
            accessBuilder.Build(),
            user,
            createQuery,
            new AppActionFriendlyName(friendlyName, ActionName).Value,
            throttledLogPathBuilder.Build(actionPath),
            scheduledBuilder.Build()
        );
    }

    IAppApiAction IAppApiActionBuilder.Build() => Build();
}
