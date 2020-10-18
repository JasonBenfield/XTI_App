using NUnit.Framework;
using System.Threading.Tasks;
using XTI_App;

namespace XTI_App.Tests
{
    public sealed class NewVersionCommandTest
    {
        [Test]
        public async Task ShouldCreateNewPatch()
        {
            var tester = await setup();
            tester.Options.VersionType = AppVersionType.Values.Patch.DisplayText;
            var newVersion = await tester.Execute();
            Assert.That(newVersion?.IsPatch(), Is.True, "Should start new patch");
        }

        [Test]
        public async Task ShouldCreateNewMinorVersion()
        {
            var tester = await setup();
            tester.Options.VersionType = AppVersionType.Values.Minor.DisplayText;
            var newVersion = await tester.Execute();
            Assert.That(newVersion?.IsMinor(), Is.True, "Should start new minor version");
        }

        [Test]
        public async Task ShouldCreateNewMajorVersion()
        {
            var tester = await setup();
            tester.Options.VersionType = AppVersionType.Values.Major.DisplayText;
            var newVersion = await tester.Execute();
            Assert.That(newVersion?.IsMajor(), Is.True, "Should start new major version");
        }

        private async Task<ManageVersionTester> setup()
        {
            var tester = new ManageVersionTester();
            await tester.Setup();
            tester.Options.Command = "New";
            return tester;
        }
    }
}
