using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface ISessionContext
    {
        Task StartSession();
        AppSession CurrentSession { get; }
        Task StartRequest();
        AppRequest CurrentRequest { get; }
    }
}
