using System.Threading.Tasks;

namespace XTI_App.Abstractions
{
    public interface IAppSetup
    {
        Task Run(AppVersionKey versionKey);
    }
}
