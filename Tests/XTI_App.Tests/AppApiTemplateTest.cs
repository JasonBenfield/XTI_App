using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTI_App.Api;
using XTI_App.TestFakes;

namespace XTI_App.Tests
{
    public sealed class AppApiTemplateTest
    {
        [Test]
        public void ShouldIncludeNumericValueTemplate()
        {
            var templateFactory = new FakeAppApiTemplateFactory();
            var template = templateFactory.Create();
            var numericValueTemplates = template.NumericValueTemplates();
            Assert.That(numericValueTemplates.Any(), Is.True, "Should include numeric value template");
        }

        [Test]
        public void ShouldIncludeNumericValuesWithTemplate()
        {
            var templateFactory = new FakeAppApiTemplateFactory();
            var template = templateFactory.Create();
            var numericValueTemplate = template.NumericValueTemplates()
                .First(templ => templ.DataType.Equals(typeof(EmployeeType)));
            var employeeTypes = EmployeeType.Values.All();
            Assert.That(numericValueTemplate.Values, Is.EquivalentTo(employeeTypes), "Should include numeric values with template");
        }
    }
}
