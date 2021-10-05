using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_Configuration.Extensions;
using XTI_Forms;

namespace XTI_App.Tests
{
    public sealed class AppApiActionTest
    {
        [Test]
        public void ShouldValidateForm()
        {
            var services = setup();
            var api = services.GetService<FakeAppApi>();
            var form = new FakeForm();
            form.TestText.SetValue(null);
            var ex = Assert.ThrowsAsync<ValidationFailedException>(() => api.Employee.SubmitFakeForm.Execute(form));
            var errors = ex.Errors.ToArray();
            Assert.That(ex.Errors.Length, Is.EqualTo(1));
            Assert.That(ex.Errors[0].Message, Is.EqualTo(FormErrors.MustNotBeNull));
        }

        [Test]
        public async Task ShouldExecuteAction_WhenFormIsValid()
        {
            var services = setup();
            var api = services.GetService<FakeAppApi>();
            var form = new FakeForm();
            form.TestText.SetValue("Valid");
            var result = await api.Employee.SubmitFakeForm.Execute(form);
            Assert.That(result.Data, Is.EqualTo("Valid"));
        }

        private IServiceProvider setup()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration
                (
                    (hostContext, config) =>
                    {
                        config.UseXtiConfiguration(hostContext.HostingEnvironment, new string[] { });
                    }
                )
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddServicesForTests(hostContext.Configuration);
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            return scope.ServiceProvider;
        }
    }
}
