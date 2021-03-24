using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.Tests
{
    public sealed class AppVersionTest
    {
        [Test]
        public async Task ShouldStartNewVersionForApp()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var version = await app.StartNewPatch(clock.Now());
            var versions = (await app.Versions()).ToArray();
            Assert.That(versions.Length, Is.EqualTo(2), "Should add version to app");
            Assert.That(versions[1].ID, Is.EqualTo(version.ID));
        }

        [Test]
        public async Task ShouldSetStatusToPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var version = await app.StartNewPatch(clock.Now());
            await version.Publishing();
            Assert.That(version.IsPublishing(), Is.True, "Should set status to publishing");
        }

        [Test]
        public async Task ShouldSetStatusToCurrent_WhenPublished()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var version = await app.StartNewPatch(clock.Now());
            await version.Publishing();
            await version.Published();
            Assert.That(version.IsCurrent(), Is.True, "Should set status to current when published");
        }

        [Test]
        public async Task ShouldOnlyAllowOneCurrentVersion()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var originalCurrent = await app.StartNewPatch(clock.Now());
            await originalCurrent.Publishing();
            await originalCurrent.Published();
            var current = await app.StartNewPatch(clock.Now());
            await current.Publishing();
            await current.Published();
            Assert.That(originalCurrent.IsCurrent(), Is.False, "Should only allow one current version");
        }

        [Test]
        public async Task ShouldAssignVersionNumber_WhenPatchBeginsPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var patch = await app.StartNewPatch(clock.Now());
            await patch.Publishing();
            Assert.That(patch.Version().Major, Is.EqualTo(1), "Should assign version number for new patch");
            Assert.That(patch.Version().Minor, Is.EqualTo(0), "Should assign version number for new patch");
            Assert.That(patch.Version().Build, Is.EqualTo(1), "Should assign version number for new patch");
        }

        [Test]
        public async Task ShouldAssignVersionNumber_WhenMinorVersionBeginsPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var minorVersion = await app.StartNewMinorVersion(clock.Now());
            await minorVersion.Publishing();
            Assert.That(minorVersion.Version().Major, Is.EqualTo(1), "Should assign version number for new minor version");
            Assert.That(minorVersion.Version().Minor, Is.EqualTo(1), "Should assign version number for new minor version");
            Assert.That(minorVersion.Version().Build, Is.EqualTo(0), "Should assign version number for new minor version");
        }

        [Test]
        public async Task ShouldAssignVersionNumber_WhenMajorVersionBeginsPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var majorVersion = await app.StartNewMajorVersion(clock.Now());
            await majorVersion.Publishing();
            Assert.That(majorVersion.Version().Major, Is.EqualTo(2), "Should assign version number for new major version");
            Assert.That(majorVersion.Version().Minor, Is.EqualTo(0), "Should assign version number for new major version");
            Assert.That(majorVersion.Version().Build, Is.EqualTo(0), "Should assign version number for new major version");
        }

        [Test]
        public async Task ShouldIncrementPatchOfCurrent_WhenPatchBeginsPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var originalCurrent = await app.StartNewPatch(clock.Now());
            await originalCurrent.Publishing();
            await originalCurrent.Published();
            var patch = await app.StartNewPatch(clock.Now());
            await patch.Publishing();
            Assert.That(patch.Version().Major, Is.EqualTo(1), "Should increment patch of current version");
            Assert.That(patch.Version().Minor, Is.EqualTo(0), "Should increment patch of current version");
            Assert.That(patch.Version().Build, Is.EqualTo(2), "Should increment patch of current version");
        }

        [Test]
        public async Task ShouldIncrementMinorVersionOfCurrent_WhenMinorVersionBeginsPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var originalCurrent = await app.StartNewMinorVersion(clock.Now());
            await originalCurrent.Publishing();
            await originalCurrent.Published();
            var minorVersion = await app.StartNewMinorVersion(clock.Now());
            await minorVersion.Publishing();
            Assert.That(minorVersion.Version().Major, Is.EqualTo(1), "Should increment minor of current version");
            Assert.That(minorVersion.Version().Minor, Is.EqualTo(2), "Should increment minor of current version");
            Assert.That(minorVersion.Version().Build, Is.EqualTo(0), "Should increment minor of current version");
        }

        [Test]
        public async Task ShouldIncrementMajorVersionOfCurrent_WhenMajorVersionBeginsPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var originalCurrent = await app.StartNewMajorVersion(clock.Now());
            await originalCurrent.Publishing();
            await originalCurrent.Published();
            var majorVersion = await app.StartNewMajorVersion(clock.Now());
            await majorVersion.Publishing();
            Assert.That(majorVersion.Version().Major, Is.EqualTo(3), "Should increment major of current version");
            Assert.That(majorVersion.Version().Minor, Is.EqualTo(0), "Should increment major of current version");
            Assert.That(majorVersion.Version().Build, Is.EqualTo(0), "Should increment major of current version");
        }

        [Test]
        public async Task ShouldRetainMajorVersion_WhenMinorVersionBeginsPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var majorVersion = await app.StartNewMajorVersion(clock.Now());
            await majorVersion.Publishing();
            await majorVersion.Published();
            var minorVersion = await app.StartNewMinorVersion(clock.Now());
            await minorVersion.Publishing();
            await minorVersion.Published();
            Assert.That(minorVersion.Version().Major, Is.EqualTo(2), "Should retain major version from the previous current");
            Assert.That(minorVersion.Version().Minor, Is.EqualTo(1), "Should increment minor version");
            Assert.That(minorVersion.Version().Build, Is.EqualTo(0), "Should reset patch");
        }

        [Test]
        public async Task ShouldRetainMajorAndMinorVersion_WhenPatchBeginsPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var majorVersion = await app.StartNewMajorVersion(clock.Now());
            await majorVersion.Publishing();
            await majorVersion.Published();
            var minorVersion = await app.StartNewMinorVersion(clock.Now());
            await minorVersion.Publishing();
            await minorVersion.Published();
            var patch = await app.StartNewPatch(clock.Now());
            await patch.Publishing();
            Assert.That(patch.Version().Major, Is.EqualTo(2), "Should retain major version from the previous current");
            Assert.That(patch.Version().Minor, Is.EqualTo(1), "Should retain minor version from the previous current");
            Assert.That(patch.Version().Build, Is.EqualTo(1), "Should increment patch");
        }

        [Test]
        public async Task ShouldResetPatch_WhenMinorVersionBeginsPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var patch = await app.StartNewPatch(clock.Now());
            await patch.Publishing();
            await patch.Published();
            var minorVersion = await app.StartNewMinorVersion(clock.Now());
            await minorVersion.Publishing();
            Assert.That(minorVersion.Version().Major, Is.EqualTo(1), "Should reset patch when minor version is publishing");
            Assert.That(minorVersion.Version().Minor, Is.EqualTo(1), "Should reset patch when minor version is publishing");
            Assert.That(minorVersion.Version().Build, Is.EqualTo(0), "Should reset patch when minor version is publishing");
        }

        [Test]
        public async Task ShouldResetPatchAndMinorVersion_WhenMajorVersionBeginsPublishing()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var patch = await app.StartNewPatch(clock.Now());
            await patch.Publishing();
            await patch.Published();
            var minorVersion = await app.StartNewMinorVersion(clock.Now());
            await minorVersion.Publishing();
            await minorVersion.Published();
            var majorVersion = await app.StartNewMajorVersion(clock.Now());
            await majorVersion.Publishing();
            Assert.That(majorVersion.Version().Major, Is.EqualTo(2), "Should reset minor version and patch when major version is publishing");
            Assert.That(majorVersion.Version().Minor, Is.EqualTo(0), "Should reset minor version and patch when major version is publishing");
            Assert.That(majorVersion.Version().Build, Is.EqualTo(0), "Should reset minor version and patch when major version is publishing");
        }

        [Test]
        public async Task ShouldGetCurrentVersion_WhenVersionKeyIsCurrent()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var clock = services.GetService<Clock>();
            var patch = await app.StartNewPatch(clock.Now());
            await patch.Publishing();
            await patch.Published();
            var currentVersion = await app.Version(AppVersionKey.Current);
            Assert.That(currentVersion.ID, Is.EqualTo(patch.ID), "Should get current version when version key is current");
        }

        private async Task<IServiceProvider> setup()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddServicesForTests();
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            await sp.Setup();
            return sp;
        }

        private sealed class TestInput
        {
            public TestInput(AppFactory factory, FakeClock clock, App app)
            {
                Factory = factory;
                Clock = clock;
                App = app;
            }

            public AppFactory Factory { get; }
            public FakeClock Clock { get; }
            public App App { get; }
        }
    }
}
