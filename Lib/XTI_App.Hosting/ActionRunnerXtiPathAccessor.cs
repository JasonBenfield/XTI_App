using Microsoft.Extensions.Hosting;
using System.IO;
using XTI_App.Abstractions;

namespace XTI_App.Hosting
{
    public sealed class ActionRunnerXtiPathAccessor : IXtiPathAccessor
    {
        private readonly AppKey appKey;
        private readonly IHostEnvironment hostEnv;
        private string groupName;
        private string actionName;

        public ActionRunnerXtiPathAccessor(AppKey appKey, IHostEnvironment hostEnv)
        {
            this.appKey = appKey;
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
            if (hostEnv.IsProduction())
            {
                var appDir = new DirectoryInfo(hostEnv.ContentRootPath);
                versionKey = AppVersionKey.Parse(appDir.Name);
            }
            else
            {
                versionKey = AppVersionKey.Current;
            }
            return new XtiPath(appKey.Name)
                .WithVersion(versionKey)
                .WithGroup(groupName)
                .WithAction(actionName);
        }
    }
}
