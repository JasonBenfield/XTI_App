using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_TempLog;

namespace XTI_ODataQuery.Api;

public sealed class QueryApiActionBuilder<TArgs, TEntity> : IAppApiActionBuilder
{
    private readonly IServiceProvider sp;
    private readonly XtiPath groupPath;
    private readonly IAppApiUser user;
    private string friendlyName = "";
    private readonly ThrottledLogPathBuilder pathBuilder = new();
    private ResourceAccess access;
    private Func<QueryAction<TArgs, TEntity>> createQuery;
    private QueryApiAction<TArgs, TEntity>? builtAction;

    internal QueryApiActionBuilder
    (
        IServiceProvider sp, 
        XtiPath groupPath, 
        IAppApiUser user, 
        ResourceAccess access,
        string actionName
    )
    {
        this.sp = sp;
        this.groupPath = groupPath;
        this.user = user;
        this.access = access;
        ActionName = actionName;
        createQuery = () => new EmptyQueryAction<TArgs, TEntity>();
    }

    public string ActionName { get; }

    public QueryApiActionBuilder<TArgs, TEntity> WithFriendlyName(string friendlyName)
    {
        this.friendlyName = friendlyName;
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> WithAccess(ResourceAccess access)
    {
        this.access = access;
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> WithAllowed(params AppRoleName[] roleNames)
    {
        access = access.WithAllowed(roleNames);
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> WithoutAllowed(params AppRoleName[] roleNames)
    {
        access = access.WithoutAllowed(roleNames);
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
        new(this, pathBuilder.Requests());

    public ActionThrottledLogIntervalBuilder<QueryApiActionBuilder<TArgs, TEntity>> ThrottleExceptionLogging() =>
        new(this, pathBuilder.Exceptions());

    public QueryApiAction<TArgs, TEntity> Build()
    {
        builtAction ??= BuildAction();
        return builtAction;
    }

    private QueryApiAction<TArgs, TEntity> BuildAction()
    {
        var actionPath = groupPath.WithAction(ActionName);
        pathBuilder.Path(actionPath.Value());
        return new QueryApiAction<TArgs, TEntity>
        (
            actionPath,
            access,
            user,
            createQuery,
            new AppActionFriendlyName(friendlyName, ActionName).Value,
            pathBuilder.Build()
        );
    }

    IAppApiAction IAppApiActionBuilder.Build() => Build();
}
