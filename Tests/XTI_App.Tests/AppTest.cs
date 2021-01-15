using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_App.TestFakes;
using XTI_Core.Fakes;

namespace XTI_App.Tests
{
    public sealed class AppTest
    {
        [Test]
        public async Task ShouldConvertToAppModel()
        {
            var input = await setup();
            var appModel = input.App.ToAppModel();
            Assert.That(appModel.ID, Is.EqualTo(input.App.ID.Value));
            Assert.That(appModel.AppName, Is.EqualTo("Fake"));
            Assert.That(appModel.Title, Is.EqualTo("Fake Title"));
            Assert.That(appModel.Type, Is.EqualTo(FakeAppKey.AppKey.Type));
        }

        private async Task<TestInput> setup()
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
            var factory = sp.GetService<AppFactory>();
            var clock = sp.GetService<FakeClock>();
            var setup = new FakeAppSetup(factory, clock);
            await setup.Run();
            var app = await factory.Apps().App(FakeAppKey.AppKey);
            return new TestInput(sp, app);
        }

        private sealed class TestInput
        {
            public TestInput(IServiceProvider sp, App app)
            {
                Factory = sp.GetService<AppFactory>();
                Clock = sp.GetService<FakeClock>();
                App = app;
            }

            public AppFactory Factory { get; }
            public FakeClock Clock { get; }
            public App App { get; }
        }
    }
}
