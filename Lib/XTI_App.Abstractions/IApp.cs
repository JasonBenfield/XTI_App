using System.Collections.Generic;
using System.Threading.Tasks;

namespace XTI_App.Abstractions
{
    public interface IApp
    {
        EntityID ID { get; }
        string Title { get; }
        Task<IAppVersion> Version(AppVersionKey versionKey);
        Task<IEnumerable<IAppRole>> Roles();
    }
}
