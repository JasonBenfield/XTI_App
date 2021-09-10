using System.Threading.Tasks;
using XTI_App.Api;
using XTI_App.Tests;

namespace XTI_App.Fakes
{
    public sealed class SubmitFakeFormAction : AppAction<FakeForm, string>
    {
        public Task<string> Execute(FakeForm model)
        {
            return Task.FromResult(model.TestText.Value());
        }
    }
}
