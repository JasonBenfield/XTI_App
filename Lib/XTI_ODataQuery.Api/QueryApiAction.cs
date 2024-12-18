using Microsoft.AspNetCore.OData.Query;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;
using XTI_TempLog;

namespace XTI_ODataQuery.Api;

public sealed class QueryApiAction<TRequest, TEntity> : IAppApiAction
{
    private readonly IAppApiUser user;
    private readonly Func<QueryAction<TRequest, TEntity>> createQuery;
    private readonly ThrottledLogXtiPath throttledLogPath;

    public QueryApiAction
    (
        XtiPath path,
        ResourceAccess access,
        IAppApiUser user,
        Func<QueryAction<TRequest, TEntity>> createQuery,
        string friendlyName,
        ThrottledLogXtiPath throttledLogPath,
        ScheduledAppAgendaItemOptions schedule
    )
    {
        path.EnsureActionResource();
        Access = access;
        Path = path;
        Schedule = schedule;
        FriendlyName = string.IsNullOrWhiteSpace(friendlyName)
            ? string.Join(" ", new CamelCasedWord(path.Action.DisplayText).Words())
            : friendlyName;
        this.user = user;
        this.createQuery = createQuery;
        this.throttledLogPath = throttledLogPath;
    }

    public XtiPath Path { get; }
    public string ActionName { get => Path.Action.DisplayText.Replace(" ", ""); }
    public string FriendlyName { get; }
    public ResourceAccess Access { get; }
    public ScheduledAppAgendaItemOptions Schedule { get; }
    public RequestDataLoggingTypes RequestDataLoggingType { get; } = RequestDataLoggingTypes.Never;
    public bool IsResultDataLoggingEnabled { get; } = false;

    public ThrottledLogPath ThrottledLogPath(XtiBasePath xtiBasePath) => throttledLogPath.Value(xtiBasePath);

    public async Task<IQueryable<TEntity>> Execute(ODataQueryOptions<TEntity> options, TRequest requestData, CancellationToken stoppingToken = default)
    {
        await user.EnsureUserHasAccess(Path);
        var queryAction = createQuery();
        var queryable = await queryAction.Execute(options, requestData);
        return queryable;
    }

    public AppApiActionTemplate Template()
    {
        var requestTemplate = new ValueTemplateFromType(typeof(TRequest)).Template();
        var resultTemplate = new QueryableValueTemplate(typeof(IQueryable<TEntity>));
        return new AppApiActionTemplate
        (
            Path.Action.DisplayText, 
            FriendlyName, 
            Access, 
            requestTemplate, 
            resultTemplate
        );
    }

    public override string ToString() => $"{GetType().Name} {FriendlyName}";
}
