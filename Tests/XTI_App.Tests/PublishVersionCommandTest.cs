using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_Version;

namespace XTI_App.Tests
{
    public sealed class PublishVersionCommandTest
    {
        [Test]
        public async Task ShouldRequireValidVersionType()
        {
            var tester = await setup();
            tester.Options.VersionType = "Whatever";
            Assert.ThrowsAsync<PublishVersionException>(() => tester.Execute());
        }

        [Test]
        public async Task ShouldBeginPublishingTheVersion()
        {
            var tester = await setup();
            await tester.Execute();
            var versions = await tester.App.Versions();
            var newVersion = versions.First(v => !v.IsCurrent());
            await tester.Checkout(newVersion);
            tester.Options.CommandBeginPublish();
            await tester.Execute();
            newVersion = await tester.App.Version(newVersion.Key());
            Assert.That(newVersion.IsPublishing(), Is.True, "Should begin publishing the new version");
        }

        [Test]
        public async Task EndPublishShouldMakeTheVersionCurrent()
        {
            var tester = await setup();
            await tester.Execute();
            var versions = await tester.App.Versions();
            var newVersion = versions.First(v => !v.IsCurrent());
            await tester.Checkout(newVersion);
            tester.Options.CommandBeginPublish();
            await tester.Command().Execute(tester.Options);
            tester.Options.CommandCompleteVersion("JasonBenfield", "XTI_App");
            await tester.Execute();
            newVersion = await tester.App.Version(newVersion.Key());
            Assert.That(newVersion.IsCurrent(), Is.True, "Should make the new version the current version");
        }

        [Test]
        public async Task ShouldNotAllowAPublishedVersionToGoBackToPublishing()
        {
            var tester = await setup();
            await tester.Execute();
            var versions = await tester.App.Versions();
            var newVersion = versions.First(v => !v.IsCurrent());
            await tester.Checkout(newVersion);
            tester.Options.CommandBeginPublish();
            await tester.Execute();
            tester.Options.CommandCompleteVersion("JasonBenfield", "XTI_App");
            await tester.Execute();
            await tester.Checkout(newVersion);
            tester.Options.CommandBeginPublish();
            Assert.ThrowsAsync<PublishVersionException>(() => tester.Execute());
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
