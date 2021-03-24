using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public interface IAppApiGroup
    {
        XtiPath Path { get; }
        ResourceAccess Access { get; }
        Task<bool> HasAccess();
        IEnumerable<IAppApiAction> Actions();
        AppApiAction<TModel, TResult> Action<TModel, TResult>(string actionName);
        AppApiGroupTemplate Template();
    }
}
