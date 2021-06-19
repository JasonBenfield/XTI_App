namespace XTI_VersionToolApi
{
    public sealed class VersionToolOptions
    {
        public string Command { get; set; }
        public string AppName { get; set; }
        public string AppType { get; set; }
        public string VersionType { get; set; }
        public string VersionKey { get; set; }

        public void CommandNew(string appName, string appType, string versionType)
        {
            Command = "New";
            AppName = appName;
            AppType = appType;
            VersionType = versionType;
        }

        public void CommandBeginPublish(string versionKey)
        {
            Command = "BeginPublish";
            VersionKey = versionKey;
        }

        public void CommandEndPublish(string versionKey)
        {
            Command = "EndPublish";
            VersionKey = versionKey;
        }

        public void CommandGetCurrent(string appName, string appType)
        {
            Command = "GetCurrent";
            AppName = appName;
            AppType = appType;
        }

        public void CommandGetVersion(string versionKey)
        {
            Command = "GetVersion";
            VersionKey = versionKey;
        }
    }
}
