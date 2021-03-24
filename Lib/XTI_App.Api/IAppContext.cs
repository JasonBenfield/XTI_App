using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public interface IAppContext
    {
        Task<IApp> App();
        Task<IAppVersion> Version();
    }
    public interface ISourceAppContext : IAppContext
    {
    }
}
