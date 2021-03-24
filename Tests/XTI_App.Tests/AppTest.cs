using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_App.TestFakes;

namespace XTI_App.Tests
{
    public sealed class AppTest
    {
        [Test]
        public async Task ShouldConvertToAppModel()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var appModel = app.ToAppModel();
            Assert.That(appModel.ID, Is.EqualTo(app.ID.Value));
            Assert.That(appModel.AppName, Is.EqualTo("Fake"));
            Assert.That(appModel.Title, Is.EqualTo("Fake Title"));
            Assert.That(appModel.Type, Is.EqualTo(FakeInfo.AppKey.Type));
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
            var factory = sp.GetService<AppFactory>();
            await sp.Setup();
            return sp;
        }
    }
}
