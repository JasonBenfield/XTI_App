using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.TestFakes;

namespace XTI_App.Tests
{
    public class ModifierCategoryTest
    {
        [Test]
        public async Task ShouldGetResourceGroups()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var departmentCategory = await app.ModCategory(FakeInfo.ModCategories.Department);
            var resourceGroups = await departmentCategory.ResourceGroups();
            Assert.That
            (
                resourceGroups.Select(rg => rg.Name()),
                Is.EquivalentTo(new[] { new ResourceGroupName("Employee") })
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
