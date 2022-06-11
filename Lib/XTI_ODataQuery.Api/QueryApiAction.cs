using Microsoft.AspNetCore.OData.Query;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;

namespace XTI_ODataQuery.Api;

public sealed class QueryApiAction<TEntity> : IAppApiAction
{
    private readonly IAppApiUser user;
    private readonly Func<QueryAction<TEntity>> createQuery;

    public QueryApiAction
    (
        XtiPath path,
        ResourceAccess access,
        IAppApiUser user,
        Func<QueryAction<TEntity>> createQuery,
        string friendlyName
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
    }

    public XtiPath Path { get; }
    public string ActionName { get => Path.Action.DisplayText.Replace(" ", ""); }
    public string FriendlyName { get; }
    public ResourceAccess Access { get; }

    public IQueryable<TEntity> Execute(ODataQueryOptions options, CancellationToken stoppingToken = default)
    {
        user.EnsureUserHasAccess(Access).Wait(60000, stoppingToken);
        var query = createQuery();
        return query.Execute(options);
    }

    public AppApiActionTemplate Template()
    {
        var modelTemplate = new ValueTemplateFromType(typeof(string)).Template();
        var resultTemplate = new ValueTemplateFromType(typeof(IQueryable<TEntity>)).Template();
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
