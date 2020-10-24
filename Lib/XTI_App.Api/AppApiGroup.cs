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
            ModifierCategoryName modCategory,
            ResourceAccess access,
            IAppApiUser user,
            Func<XtiPath, ResourceAccess, IAppApiUser, IAppApiActionCollection> createActions
        )
        {
            Path = api.Path.WithGroup(groupName);
            this.modCategory = modCategory ?? ModifierCategoryName.Default;
            Access = access ?? ResourceAccess.AllowAuthenticated();
            this.user = user;
            actions = createActions(Path, Access, user);
        }

        private readonly ModifierCategoryName modCategory;
        private readonly IAppApiUser user;
        private readonly IAppApiActionCollection actions;

        protected T Actions<T>() where T : IAppApiActionCollection => (T)actions;

        protected void Actions<T>(Action<T> init) where T : IAppApiActionCollection
        {
            init((T)actions);
        }

        public XtiPath Path { get; }
        public ResourceAccess Access { get; }

        public Task<bool> HasAccess() => HasAccess(ModifierKey.Default);
        public Task<bool> HasAccess(ModifierKey modifier) => user.HasAccess(Path, Access, modifier);

        public IEnumerable<IAppApiAction> Actions() => actions.Actions();

        public AppApiAction<TModel, TResult> Action<TModel, TResult>(string actionName) =>
            actions.Action<TModel, TResult>(actionName);

        public AppApiGroupTemplate Template()
        {
            var actionTemplates = Actions().Select(a => a.Template());
            return new AppApiGroupTemplate(Path.Group.DisplayText, modCategory, Access, actionTemplates);
        }
    }
}
