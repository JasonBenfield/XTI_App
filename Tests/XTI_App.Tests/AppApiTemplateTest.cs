using NUnit.Framework;
using System.Linq;
using XTI_App.TestFakes;

namespace XTI_App.Tests
{
    public sealed class AppApiTemplateTest
    {
        [Test]
        public void ShouldIncludeNumericValueTemplate()
        {
            var apiFactory = new FakeAppApiFactory();
            var template = apiFactory.CreateTemplate();
            var numericValueTemplates = template.NumericValueTemplates();
            Assert.That(numericValueTemplates.Any(), Is.True, "Should include numeric value template");
        }

        [Test]
        public void ShouldIncludeNumericValuesWithTemplate()
        {
            var apiFactory = new FakeAppApiFactory();
            var template = apiFactory.CreateTemplate();
            var numericValueTemplate = template.NumericValueTemplates()
                .First(templ => templ.DataType.Equals(typeof(EmployeeType)));
            var employeeTypes = EmployeeType.Values.All();
            Assert.That(numericValueTemplate.Values, Is.EquivalentTo(employeeTypes), "Should include numeric values with template");
        }
    }
}
