using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_Configuration.Extensions;

namespace XTI_App.Tests;

internal sealed class AppApiTemplateTest
{
    [Test]
    public void ShouldIncludeNumericValueTemplate()
    {
        var services = setup();
        var template = services.GetRequiredService<AppApiFactory>().CreateTemplate();
        var numericValueTemplates = template.NumericValueTemplates();
        Assert.That(numericValueTemplates.Any(), Is.True, "Should include numeric value template");
    }

    [Test]
    public void ShouldIncludeNumericValuesWithTemplate()
    {
        var services = setup();
        var template = services.GetRequiredService<AppApiFactory>().CreateTemplate();
        var numericValueTemplate = template.NumericValueTemplates()
            .First(templ => templ.DataType.Equals(typeof(EmployeeType)));
        var employeeTypes = EmployeeType.Values.All();
        Assert.That(numericValueTemplate.Values, Is.EquivalentTo(employeeTypes), "Should include numeric values with template");
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