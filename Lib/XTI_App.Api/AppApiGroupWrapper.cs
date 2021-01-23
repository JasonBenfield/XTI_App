using System.Collections.Generic;
using System.Threading.Tasks;

namespace XTI_App.Api
{
    public class AppApiGroupWrapper : IAppApiGroup
    {
        protected readonly AppApiGroup source;

        protected AppApiGroupWrapper(AppApiGroup source)
        {
            this.source = source;
        }

        public XtiPath Path { get => source.Path; }
        public ResourceAccess Access { get => source.Access; }
        public Task<bool> HasAccess() => source.HasAccess();
        public IEnumerable<IAppApiAction> Actions() => source.Actions();
        public AppApiAction<TModel, TResult> Action<TModel, TResult>(string actionName)
            => source.Action<TModel, TResult>(actionName);
        public AppApiGroupTemplate Template() => source.Template();
    }
}
