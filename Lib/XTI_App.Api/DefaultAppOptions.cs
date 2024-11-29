using XTI_Core;
using XTI_TempLog;

namespace XTI_App.Api;

public class DefaultAppOptions
{
    public string VersionKey { get; set; } = "";
    public XtiTokenOptions XtiToken { get; set; } = new();
    public HubClientOptions HubClient { get; set; } = new();
    public DbOptions DB { get; set; } = new();
    public TempLogOptions TempLog { get; set; } = new();
}
