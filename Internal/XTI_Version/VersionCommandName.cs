using System;
using System.Linq;
using XTI_Core;

namespace XTI_Version
{
    public sealed class VersionCommandName : TextValue
    {
        public static VersionCommandName NewVersion = new VersionCommandName(nameof(NewVersion));
        public static VersionCommandName NewIssue = new VersionCommandName(nameof(NewIssue));
        public static VersionCommandName Issues = new VersionCommandName(nameof(Issues));
        public static VersionCommandName StartIssue = new VersionCommandName(nameof(StartIssue));
        public static VersionCommandName CompleteIssue = new VersionCommandName(nameof(CompleteIssue));
        public static VersionCommandName GetCurrentVersion = new VersionCommandName(nameof(GetCurrentVersion));
        public static VersionCommandName GetVersion = new VersionCommandName(nameof(GetVersion));
        public static VersionCommandName BeginPublish = new VersionCommandName(nameof(BeginPublish));
        public static VersionCommandName CompleteVersion = new VersionCommandName(nameof(CompleteVersion));

        public static readonly VersionCommandName[] All = new[]
        {
            NewVersion,
            NewIssue,
            Issues,
            StartIssue,
            CompleteIssue,
            GetCurrentVersion,
            GetVersion,
            BeginPublish,
            CompleteVersion
        };

        public static VersionCommandName FromValue(string value)
        {
            value = value?.Replace(" ", "") ?? "";
            var command = All.FirstOrDefault(c => c.Equals(value));
            if (command == null)
            {
                throw new ArgumentException($"Command '{value}' was not found");
            }
            return command;
        }

        private VersionCommandName(string value)
            : base(value)
        {
        }
    }
}
