using System;
using XTI_App;
using XTI_Core;

namespace XTI_Version
{
    public sealed class VersionCommandFactory
    {
        private readonly AppFactory appFactory;
        private readonly Clock clock;
        private readonly GitFactory gitFactory;

        public VersionCommandFactory(AppFactory appFactory, Clock clock, GitFactory gitFactory)
        {
            this.appFactory = appFactory;
            this.clock = clock;
            this.gitFactory = gitFactory;
        }

        public VersionCommand Create(VersionCommandName commandName)
        {
            VersionCommand command;
            if (commandName.Equals(VersionCommandName.NewVersion))
            {
                command = new NewVersionCommand(appFactory, clock, gitFactory);
            }
            else if (commandName.Equals(VersionCommandName.NewIssue))
            {
                command = new NewIssueCommand(gitFactory);
            }
            else if (commandName.Equals(VersionCommandName.Issues))
            {
                command = new IssuesCommand(gitFactory);
            }
            else if (commandName.Equals(VersionCommandName.StartIssue))
            {
                command = new StartIssueCommand(gitFactory);
            }
            else if (commandName.Equals(VersionCommandName.CompleteIssue))
            {
                command = new CompleteIssueCommand(gitFactory);
            }
            else if (commandName.Equals(VersionCommandName.GetCurrentVersion))
            {
                command = new GetCurrentVersionCommand(appFactory, gitFactory);
            }
            else if (commandName.Equals(VersionCommandName.GetVersion))
            {
                command = new GetVersionCommand(appFactory, gitFactory);
            }
            else if (commandName.Equals(VersionCommandName.BeginPublish))
            {
                command = new BeginPublishCommand(appFactory, gitFactory);
            }
            else if (commandName.Equals(VersionCommandName.CompleteVersion))
            {
                command = new CompleteVersionCommand(appFactory, gitFactory);
            }
            else
            {
                throw new NotSupportedException($"Command '{commandName.Value}' is not supported");
            }
            return command;
        }

    }
}
