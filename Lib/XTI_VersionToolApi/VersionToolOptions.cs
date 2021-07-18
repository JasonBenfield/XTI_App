namespace XTI_VersionToolApi
{
    public sealed class VersionToolOptions
    {
        public string Command { get; set; }
        public string AppName { get; set; }
        public string AppType { get; set; }
        public string VersionType { get; set; }
        public string RepoOwner { get; set; }
        public string RepoName { get; set; }
        public string IssueTitle { get; set; }
        public int IssueNumber { get; set; }
        public string OutputPath { get; set; }
        public bool StartIssue { get; set; }

        public void CommandNewVersion(string appName, string appType, string versionType, string repoOwner, string repoName)
        {
            Command = "NewVersion";
            AppName = appName;
            AppType = appType;
            VersionType = versionType;
            RepoOwner = repoOwner;
            RepoName = repoName;
        }

        public void CommandBeginPublish()
        {
            Command = "BeginPublish";
        }

        public void CommandCompleteVersion(string repoOwner, string repoName)
        {
            Command = "CompleteVersion";
            RepoOwner = repoOwner;
            RepoName = repoName;
        }

        public void CommandGetCurrentVersion(string appName, string appType)
        {
            Command = "GetCurrentVersion";
            AppName = appName;
            AppType = appType;
        }

        public void CommandGetVersion()
        {
            Command = "GetVersion";
        }

        public void CommandNewIssue(string repoOwner, string repoName, string title)
        {
            Command = "NewIssue";
            RepoOwner = repoOwner;
            RepoName = repoName;
            IssueTitle = title;
        }

        public void CommandIssues(string repoOwner, string repoName)
        {
            Command = "Issues";
            RepoOwner = repoOwner;
            RepoName = repoName;
        }

        public void CommandStartIssue(string repoOwner, string repoName, int issueNumber)
        {
            Command = "StartIssue";
            RepoOwner = repoOwner;
            RepoName = repoName;
            IssueNumber = issueNumber;
        }

        public void CommandCompleteIssue(string repoOwner, string repoName)
        {
            Command = "CompleteIssue";
            RepoOwner = repoOwner;
            RepoName = repoName;
        }
    }
}
