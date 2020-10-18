using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;
using XTI_App;
using XTI_App.EF;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.Tests
{
    public sealed class AppTest
    {
        [Test]
        public async Task ShouldAddApp()
        {
            var input = await setup();
            var appKey = new AppKey("Fake");
            var title = "Fake Title";
            await input.Factory.Apps().AddApp(appKey, AppType.Values.WebApp, title, input.Clock.Now());
            var app = await input.Factory.Apps().WebApp(appKey);
            Assert.That(app.Exists(), Is.True, "Should add app");
            Assert.That(app.Title, Is.EqualTo(title), "Should set app title");
        }

        [Test]
        public async Task ShouldChangeAppTitle()
        {
            var input = await setup();
            var appKey = new AppKey("Fake");
            await input.Factory.Apps().AddApp(appKey, AppType.Values.WebApp, "Original Title", input.Clock.Now());
            var app = await input.Factory.Apps().WebApp(appKey);
            var newTitle = "New Title";
            await app.SetTitle(newTitle);
            Assert.That(app.Title, Is.EqualTo(newTitle), "Should set app title");
        }

        private async Task<TestInput> setup()
        {
            var services = new ServiceCollection();
            services.AddServicesForTests();
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<AppFactory>();
            var setup = new AppSetup(factory);
            await setup.Run();
            return new TestInput(sp);
        }

        private sealed class TestInput
        {
            public TestInput(ServiceProvider sp)
            {
                Factory = sp.GetService<AppFactory>();
                Clock = sp.GetService<FakeClock>();
            }

            public AppFactory Factory { get; }
            public FakeClock Clock { get; }
        }
    }
}
