using System.Threading.Tasks;
using XTI_Credentials;

namespace XTI_App.Secrets
{
    public interface ISystemUserCredentials
    {
        Task<CredentialValue> Value();
    }
}
