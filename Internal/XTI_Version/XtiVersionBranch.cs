using System.Text.RegularExpressions;
using XTI_App;

namespace XTI_Version
{
    public sealed class XtiVersionBranch
    {
        private readonly string branchName;
        private static readonly Regex branchNameRegex = new Regex("^xti\\/((major)|(minor)|(patch))\\/(V?\\d+)$", RegexOptions.IgnoreCase);

        public XtiVersionBranch(AppVersion version)
            : this(getBranchName(version))
        {
        }

        private static string getBranchName(AppVersion version)
        {
            var type = version.Type().DisplayText;
            return $"xti/{type}/{version.Key().Value}";
        }

        public XtiVersionBranch(string branchName)
        {
            if (branchNameRegex.IsMatch(branchName))
            {
                this.branchName = branchName;
            }
            else
            {
                throw new InvalidBranchException(branchName);
            }
        }

        public string BranchName() => branchName;

        public string VersionKey()
        {
            var match = branchNameRegex.Match(branchName);
            return match.Groups[5].Value;
        }

        public override string ToString() => $"{nameof(XtiVersionBranch)} {branchName}";
    }
}
