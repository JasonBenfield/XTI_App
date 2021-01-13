using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainDB.Extensions;
using XTI_Configuration.Extensions;
using Microsoft.Extensions.DependencyInjection;
using XTI_App.TestFakes;

namespace XTI_App.IntegrationTests
{
    public sealed class DefaultAppSetupTest
    {
        [Test]
        public async Task ShouldSetupApp()
        {
            var services = setup();
            await services.Reset();
            var fakeAppSetup = services.GetService<FakeAppSetup>();
            await fakeAppSetup.Run();
            var factory = services.GetService<AppFactory>();
            var app = await factory.Apps().App(FakeAppKey.AppKey);
            var groups = await app.ResourceGroups();
            Assert.That
            (
                groups.Select(g => g.Name()),
                Is.EquivalentTo(new[] { new ResourceGroupName("Login"), new ResourceGroupName("Home"), new ResourceGroupName("Employee"), new ResourceGroupName("Product") })
            );
            var employeeGroup = groups.First(g => g.Name().Equals("Employee"));
            var allowedGroupRoles = await employeeGroup.AllowedRoles();
            Assert.That
            (
                allowedGroupRoles.Select(r => r.Name()),
                Is.EquivalentTo(new[] { FakeAppRoles.Instance.Admin })
            );
            var resources = await employeeGroup.Resources();
            Assert.That
            (
                resources.Select(r => r.Name()),
                Is.EquivalentTo(new[] { new ResourceName("AddEmployee"), new ResourceName("Employee"), new ResourceName("SubmitFakeForm") })
            );
            var addEmployeeResource = resources.First(r => r.Name().Equals("AddEmployee"));
            var allowedResourceRoles = await addEmployeeResource.AllowedRoles();
            Assert.That
            (
                allowedResourceRoles.Select(r => r.Name()),
                Is.EquivalentTo(new[] { FakeAppRoles.Instance.Admin, FakeAppRoles.Instance.Manager })
            );
        }

        private static IServiceProvider setup()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration
                (
                    (hostContext, config)
                        => config.UseXtiConfiguration(hostContext.HostingEnvironment, new string[] { })
                )
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddXtiTestServices(hostContext.Configuration);
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            return services;
        }
    }
}
