using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_Core.Extensions;

namespace XTI_App.Tests;

internal sealed class AppApiTemplateTest
{
    [Test]
    public void ShouldIncludeNumericValueTemplate()
    {
        var services = setup();
        var template = services.GetRequiredService<AppApiFactory>().CreateTemplate();
        var numericValueTemplates = template.NumericValueTemplates(ApiCodeGenerators.Dotnet);
        Assert.That(numericValueTemplates.Any(), Is.True, "Should include numeric value template");
    }

    [Test]
    public void ShouldIncludeNumericValuesWithTemplate()
    {
        var services = setup();
        var template = services.GetRequiredService<AppApiFactory>().CreateTemplate();
        var numericValueTemplate = template.NumericValueTemplates(ApiCodeGenerators.Dotnet)
            .First(templ => templ.DataType.Equals(typeof(EmployeeType)));
        var employeeTypes = EmployeeType.Values.GetAll();
        Assert.That(numericValueTemplate.Values, Is.EquivalentTo(employeeTypes), "Should include numeric values with template");
    }

    private IServiceProvider setup()
    {
        var hostBuilder = new XtiHostBuilder();
        hostBuilder.Services.AddServicesForTests();
        return hostBuilder.Build().Scope();
    }
}