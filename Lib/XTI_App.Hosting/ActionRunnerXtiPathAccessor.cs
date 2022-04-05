using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App.Hosting;

public sealed class ActionRunnerXtiPathAccessor : IXtiPathAccessor
{
    private readonly AppKey appKey;
    private readonly XtiEnvironment xtiEnv;
    private readonly IHostEnvironment hostEnv;
    private string groupName = "";
    private string actionName = "";

    public ActionRunnerXtiPathAccessor(AppKey appKey, XtiEnvironment xtiEnv, IHostEnvironment hostEnv)
    {
        this.appKey = appKey;
        this.xtiEnv = xtiEnv;
        this.hostEnv = hostEnv;
    }

    public void FinishPath(string groupName, string actionName)
    {
        this.groupName = groupName.Replace(" ", "");
        this.actionName = actionName.Replace(" ", "");
    }

    public XtiPath Value()
    {
        AppVersionKey versionKey;
        if (xtiEnv.IsProduction())
        {
            var appDir = new DirectoryInfo(hostEnv.ContentRootPath);
            versionKey = AppVersionKey.Parse(appDir.Name);
        }
        else
        {
            versionKey = AppVersionKey.Current;
        }
        return new XtiPath(appKey.Name.DisplayText)
            .WithVersion(versionKey)
            .WithGroup(groupName)
            .WithAction(actionName);
    }
}