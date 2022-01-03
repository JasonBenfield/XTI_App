using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface ISourceAppContext : IAppContext
{
    Task<ModifierKey> ModKeyInHubApps(IApp app);
}