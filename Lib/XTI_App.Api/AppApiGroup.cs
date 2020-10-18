using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTI_App.Api
{
    public class AppApiGroup
    {
        public AppApiGroup
        (
            AppApi api,
            string groupName,
            bool hasModifier,
            ResourceAccess access,
            IAppApiUser user,
            Func<XtiPath, ResourceAccess, IAppApiUser, IAppApiActionCollection> createActions
        )
        {
            Name = api.Name.WithGroup(groupName);
            this.hasModifier = hasModifier;
            Access = access ?? ResourceAccess.AllowAuthenticated();
            this.user = user;
            this.actions = createActions(Name, Access, user);
        }

        private readonly bool hasModifier;
        private readonly IAppApiUser user;
        private readonly IAppApiActionCollection actions;

        protected T Actions<T>() where T : IAppApiActionCollection => (T)actions;

        protected void Actions<T>(Action<T> init) where T : IAppApiActionCollection
        {
            init((T)actions);
        }

        public XtiPath Name { get; }
        public ResourceAccess Access { get; }

        public Task<bool> HasAccess() => HasAccess(AccessModifier.Default);
        public Task<bool> HasAccess(AccessModifier modifier) => user.HasAccess(Access, modifier);

        public IEnumerable<IAppApiAction> Actions() => actions.Actions();

        public AppApiAction<TModel, TResult> Action<TModel, TResult>(string actionName) =>
            actions.Action<TModel, TResult>(actionName);

        public AppApiGroupTemplate Template()
        {
            var actionTemplates = Actions().Select(a => a.Template());
            return new AppApiGroupTemplate(Name.Group, hasModifier, Access, actionTemplates);
        }
    }
}
