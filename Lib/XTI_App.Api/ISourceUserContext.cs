using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface ISourceUserContext : IUserContext
    {
        Task<string> GetKey();
    }
}
