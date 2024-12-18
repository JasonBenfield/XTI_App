using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public sealed class QueryApiActionBuilder<TRequest, TEntity> : IAppApiActionBuilder
{
    private readonly IServiceProvider sp;
    private readonly XtiPath groupPath;
    private readonly IAppApiUser user;
    private string friendlyName = "";
    private readonly ActionThrottledLogPathBuilder<QueryApiActionBuilder<TRequest, TEntity>> throttledLogPathBuilder;
    private ResourceAccessBuilder accessBuilder;
    private Func<QueryAction<TRequest, TEntity>> createQuery;
    private QueryApiAction<TRequest, TEntity>? builtAction;

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
        createQuery = () => new EmptyQueryAction<TRequest, TEntity>();
    }

    public string ActionName { get; }

    public QueryApiActionBuilder<TRequest, TEntity> WithFriendlyName(string friendlyName)
    {
        this.friendlyName = friendlyName;
        return this;
    }

    public QueryApiActionBuilder<TRequest, TEntity> AllowAnonymousAccess()
    {
        accessBuilder.AllowAnonymous();
        return this;
    }

    public QueryApiActionBuilder<TRequest, TEntity> ResetAccess()
    {
        accessBuilder.Reset();
        return this;
    }

    public QueryApiActionBuilder<TRequest, TEntity> ResetAccess(ResourceAccess access)
    {
        accessBuilder.Reset(access);
        return this;
    }

    public QueryApiActionBuilder<TRequest, TEntity> ResetAccessWithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.Reset(roleNames);
        return this;
    }

    public QueryApiActionBuilder<TRequest, TEntity> WithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithAllowed(roleNames);
        return this;
    }

    public QueryApiActionBuilder<TRequest, TEntity> WithoutAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithoutAllowed(roleNames);
        return this;
    }

    public QueryApiActionBuilder<TRequest, TEntity> WithQuery<TExecution>() where TExecution : QueryAction<TRequest, TEntity>
    {
        WithQuery(() => sp.GetRequiredService<TExecution>());
        return this;
    }

    public QueryApiActionBuilder<TRequest, TEntity> WithQuery(Func<QueryAction<TRequest, TEntity>> createQuery)
    {
        this.createQuery = createQuery;
        return this;
    }

    public ActionThrottledLogIntervalBuilder<QueryApiActionBuilder<TRequest, TEntity>> ThrottleRequestLogging() =>
        throttledLogPathBuilder.RequestIntervalBuilder;

    public ActionThrottledLogIntervalBuilder<QueryApiActionBuilder<TRequest, TEntity>> ThrottleExceptionLogging() =>
        throttledLogPathBuilder.ExceptionIntervalBuilder;

    public XtiPath ActionPath() => groupPath.WithAction(ActionName);

    public QueryApiAction<TRequest, TEntity> Build()
    {
        builtAction ??= BuildAction();
        return builtAction;
    }

    private QueryApiAction<TRequest, TEntity> BuildAction()
    {
        var actionPath = ActionPath();
        var scheduledBuilder = new ActionScheduleBuilder<QueryApiActionBuilder<TRequest, TEntity>>(this);
        return new QueryApiAction<TRequest, TEntity>
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
