using System.Threading.Tasks;
using XTI_Credentials;

namespace XTI_App.Secrets
{
    public interface ICredentialsAccessor
    {
        Task<CredentialValue> Value();
    }
}
