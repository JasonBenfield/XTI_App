using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
