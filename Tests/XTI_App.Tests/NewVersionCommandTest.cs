using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Tests
{
    public sealed class NewVersionCommandTest
    {
        [Test]
        public async Task ShouldCreateNewPatch()
        {
            var tester = await setup();
            tester.Options.VersionType = AppVersionType.Values.Patch.DisplayText;
            await tester.Execute();
            var versions = await tester.App.Versions();
            var newVersion = versions.First(v => !v.IsCurrent());
            Assert.That(newVersion?.IsPatch(), Is.True, "Should start new patch");
        }

        [Test]
        public async Task ShouldCreateNewMinorVersion()
        {
            var tester = await setup();
            tester.Options.VersionType = AppVersionType.Values.Minor.DisplayText;
            await tester.Execute();
            var versions = await tester.App.Versions();
            var newVersion = versions.First(v => !v.IsCurrent());
            Assert.That(newVersion?.IsMinor(), Is.True, "Should start new minor version");
        }

        [Test]
        public async Task ShouldCreateNewMajorVersion()
        {
            var tester = await setup();
            tester.Options.VersionType = AppVersionType.Values.Major.DisplayText;
            await tester.Execute();
            var versions = await tester.App.Versions();
            var newVersion = versions.First(v => !v.IsCurrent());
            Assert.That(newVersion?.IsMajor(), Is.True, "Should start new major version");
        }

        private async Task<ManageVersionTester> setup()
        {
            var tester = new ManageVersionTester();
            await tester.Setup();
            var appKey = tester.App.Key();
            tester.Options.CommandNewVersion
            (
                appKey.Name.DisplayText,
                appKey.Type.DisplayText,
                AppVersionType.Values.Patch.DisplayText,
                "JasonBenfield",
                "XTI_App"
            );
            return tester;
        }
    }
}
