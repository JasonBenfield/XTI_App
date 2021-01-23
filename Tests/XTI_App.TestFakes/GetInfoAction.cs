using System.Threading.Tasks;
using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class GetInfoAction : AppAction<EmptyRequest, string>
    {
        public Task<string> Execute(EmptyRequest model)
        {
            return Task.FromResult("");
        }
    }

}
