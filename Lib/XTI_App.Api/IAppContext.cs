using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface IAppContext
    {
        Task<IApp> App();
        Task<IAppVersion> Version();
    }
}
