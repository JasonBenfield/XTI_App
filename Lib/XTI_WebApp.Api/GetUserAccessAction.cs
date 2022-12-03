using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class GetUserAccessAction : AppAction<ResourcePath[], ResourcePathAccess[]>
{
    private readonly CurrentUserAccess userAccess;

    public GetUserAccessAction(CurrentUserAccess userAccess)
    {
        this.userAccess = userAccess;
    }

    public async Task<ResourcePathAccess[]> Execute(ResourcePath[] paths, CancellationToken stoppingToken)
    {
        var accesses = new List<ResourcePathAccess>();
        foreach (var path in paths)
        {
            var result = await userAccess.HasAccess
            (
                new ResourceGroupName(path.Group), 
                new ResourceName(path.Action),
                new ModifierKey(path.ModKey)
            );
            accesses.Add(new ResourcePathAccess(path, result.HasAccess));
        }
        return accesses.ToArray();
    }
}
