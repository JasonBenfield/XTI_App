using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTI_App.Api
{
    public sealed class AppApiGroup : IAppApiGroup
    {
        private readonly ModifierCategoryName modCategory;
        private readonly IAppApiUser user;
        private readonly Dictionary<string, IAppApiAction> actions = new Dictionary<string, IAppApiAction>();

        public AppApiGroup
        (
            XtiPath path,
            ModifierCategoryName modCategory,
            ResourceAccess access,
            IAppApiUser user
        )
        {
            Path = path;
            this.modCategory = modCategory ?? ModifierCategoryName.Default;
            Access = access ?? ResourceAccess.AllowAuthenticated();
            this.user = user;
        }

        public IAppApiUser User { get => user; }

        public AppApiAction<TModel, TResult> Action<TModel, TResult>(string actionName) =>
            (AppApiAction<TModel, TResult>)actions[actionKey(actionName)];

        public IEnumerable<IAppApiAction> Actions() => actions.Values.ToArray();

        public AppApiAction<TModel, TResult> AddAction<TModel, TResult>(AppApiAction<TModel, TResult> action)
        {
            actions.Add(actionKey(action.ActionName), action);
            return action;
        }
        private static string actionKey(string actionName) => actionName.ToLower().Replace(" ", "");

        public XtiPath Path { get; }
        public string GroupName { get => Path.Group.DisplayText.Replace(" ", ""); }
        public ResourceAccess Access { get; }

        public Task<bool> HasAccess() => user.HasAccess(Access);

        public AppApiGroupTemplate Template()
        {
            var actionTemplates = Actions().Select(a => a.Template());
            return new AppApiGroupTemplate(Path.Group.DisplayText, modCategory, Access, actionTemplates);
        }

        internal IEnumerable<AppRoleName> RoleNames()
        {
            var roleNames = new List<AppRoleName>();
            roleNames.AddRange(Access.Allowed);
            roleNames.AddRange(Access.Denied);
            var actionRoleNames = Actions()
                .SelectMany(a => a.Access.Allowed.Union(a.Access.Denied))
                .Distinct();
            roleNames.AddRange(actionRoleNames);
            return roleNames.Distinct();
        }

        public override string ToString() => $"{GetType().Name} {Path.Group}";
    }
}
