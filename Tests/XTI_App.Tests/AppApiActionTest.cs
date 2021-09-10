using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_Forms;

namespace XTI_App.Tests
{
    public sealed class AppApiActionTest
    {
        [Test]
        public void ShouldValidateForm()
        {
            var api = new FakeAppApi(new AppApiSuperUser());
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
            var api = new FakeAppApi(new AppApiSuperUser());
            var form = new FakeForm();
            form.TestText.SetValue("Valid");
            var result = await api.Employee.SubmitFakeForm.Execute(form);
            Assert.That(result.Data, Is.EqualTo("Valid"));
        }
    }
}
