using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Extensions;

public sealed class DefaultAppClientRequestKey : IAppClientRequestKey
{
    private readonly TempLogSession tempLogSession;

    public DefaultAppClientRequestKey(TempLogSession tempLogSession)
    {
        this.tempLogSession = tempLogSession;
    }

    public string Value() => tempLogSession.GetCurrentRequestKey();
}
