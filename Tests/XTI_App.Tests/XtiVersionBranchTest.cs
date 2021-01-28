using NUnit.Framework;
using XTI_Version;

namespace XTI_App.Tests
{
    public sealed class XtiVersionBranchTest
    {
        [Test]
        public void ShouldParseBranchName()
        {
            var branch = new XtiVersionBranch("xti/minor/V1");
            Assert.That(branch.VersionType(), Is.EqualTo(AppVersionType.Values.Minor));
            Assert.That(branch.VersionKey(), Is.EqualTo(AppVersionKey.Parse("V1")));
            branch = new XtiVersionBranch("xti/Major/V3");
            Assert.That(branch.VersionType(), Is.EqualTo(AppVersionType.Values.Major));
            Assert.That(branch.VersionKey(), Is.EqualTo(AppVersionKey.Parse("V3")));
        }
    }
}
