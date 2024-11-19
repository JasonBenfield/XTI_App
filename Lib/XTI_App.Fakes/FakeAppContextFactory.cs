namespace XTI_App.Fakes;

public sealed class FakeAppContextFactory
{
    private readonly Dictionary<AppKey, FakeAppContext> appContexts = new();

    public FakeAppContext Create(AppKey appKey)
    {
        if(!appContexts.TryGetValue(appKey, out var appContext))
        {
            appContext = new FakeAppContext(appKey);
            appContexts.Add(appKey, appContext);
        }
        return appContext;
    }
}
