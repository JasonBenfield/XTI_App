using System.Threading.Tasks;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public interface VersionCommand
    {
        Task Execute(VersionToolOptions options);
    }
}
