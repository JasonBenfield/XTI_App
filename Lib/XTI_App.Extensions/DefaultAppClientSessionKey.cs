using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Extensions;

public sealed class DefaultAppClientSessionKey : IAppClientSessionKey
{
    private readonly CurrentSession currentSession;

    public DefaultAppClientSessionKey(CurrentSession currentSession)
    {
        this.currentSession = currentSession;
    }

    public string Value() => currentSession.SessionKey.Format();
}
