using Microsoft.AspNetCore.OData.Query;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;
using XTI_TempLog;

namespace XTI_ODataQuery.Api;

public sealed class QueryApiAction<TModel, TEntity> : IAppApiAction
{
    private readonly IAppApiUser user;
    private readonly Func<QueryAction<TModel, TEntity>> createQuery;
    private readonly ThrottledLogXtiPath throttledLogPath;

    public QueryApiAction
    (
        XtiPath path,
        ResourceAccess access,
        IAppApiUser user,
        Func<QueryAction<TModel, TEntity>> createQuery,
        string friendlyName,
        ThrottledLogXtiPath throttledLogPath
    )
    {
        path.EnsureActionResource();
        Access = access;
        Path = path;
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

    public ThrottledLogPath ThrottledLogPath(XtiBasePath xtiBasePath) => throttledLogPath.Value(xtiBasePath);

    public async Task<IQueryable<TEntity>> Execute(ODataQueryOptions<TEntity> options, TModel model, CancellationToken stoppingToken = default)
    {
        await user.EnsureUserHasAccess(Path);
        var queryAction = createQuery();
        var queryable = await queryAction.Execute(options, model);
        return queryable;
    }

    public AppApiActionTemplate Template()
    {
        var modelTemplate = new ValueTemplateFromType(typeof(TModel)).Template();
        var resultTemplate = new QueryableValueTemplate(typeof(IQueryable<TEntity>));
        return new AppApiActionTemplate
        (
            Path.Action.DisplayText, 
            FriendlyName, 
            Access, 
            modelTemplate, 
            resultTemplate
        );
    }

    public override string ToString() => $"{GetType().Name} {FriendlyName}";
}
