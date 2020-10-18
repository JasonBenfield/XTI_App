using NUnit.Framework;
using System.Threading.Tasks;
using XTI_Version;

namespace XTI_App.Tests
{
    public sealed class PublishVersionCommandTest
    {
        [Test]
        public async Task ShouldRequireValidBranchName()
        {
            var tester = await setup();
            var newVersion = await tester.Command().Execute(tester.Options);
            tester.Options.Command = "BeginPublish";
            tester.Options.BranchName = $"something/whatever/{newVersion.ID}";
            Assert.ThrowsAsync<InvalidBranchException>(() => tester.Execute());
        }

        [Test]
        public async Task ShouldBeginPublishingTheVersion()
        {
            var tester = await setup();
            var newVersion = await tester.Command().Execute(tester.Options);
            tester.Options.Command = "BeginPublish";
            tester.Options.BranchName = new XtiVersionBranch(newVersion).BranchName();
            var publishedVersion = await tester.Execute();
            Assert.That(publishedVersion.IsPublishing(), Is.True, "Should begin publishing the new version");
        }

        [Test]
        public async Task EndPublishShouldMakeTheVersionCurrent()
        {
            var tester = await setup();
            var newVersion = await tester.Command().Execute(tester.Options);
            tester.Options.Command = "BeginPublish";
            tester.Options.BranchName = new XtiVersionBranch(newVersion).BranchName();
            await tester.Command().Execute(tester.Options);
            tester.Options.Command = "EndPublish";
            var publishedVersion = await tester.Execute();
            Assert.That(publishedVersion.IsCurrent(), Is.True, "Should make the new version the current version");
        }

        [Test]
        public async Task ShouldNotAllowAPublishedVersionToGoBackToPublishing()
        {
            var tester = await setup();
            var newVersion = await tester.Command().Execute(tester.Options);
            tester.Options.Command = "BeginPublish";
            tester.Options.BranchName = new XtiVersionBranch(newVersion).BranchName();
            await tester.Execute();
            tester.Options.Command = "EndPublish";
            tester.Options.BranchName = new XtiVersionBranch(newVersion).BranchName();
            await tester.Execute();
            tester.Options.Command = "BeginPublish";
            tester.Options.BranchName = new XtiVersionBranch(newVersion).BranchName();
            Assert.ThrowsAsync<PublishVersionException>(() => tester.Execute());
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
