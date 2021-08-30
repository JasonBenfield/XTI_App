using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_App.TestFakes;

namespace XTI_App.Tests
{
    sealed class ModifierTest
    {
        [Test]
        public async Task ShouldGetDefaultModifier()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var departmentCategory = await app.ModCategory(FakeInfo.ModCategories.Department);
            var modifier = await departmentCategory.Modifier("IT");
            var appDefaultModifier = await app.DefaultModifier();
            var defaultModifier = await modifier.DefaultModifier();
            Assert.That(defaultModifier.ID.IsValid(), Is.True);
            Assert.That
            (
                defaultModifier.ID,
                Is.EqualTo(appDefaultModifier.ID)
            );
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
    }
}
