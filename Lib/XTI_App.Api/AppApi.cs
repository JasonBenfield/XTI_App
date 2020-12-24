using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTI_App.Api
{
    public class AppApi
    {
        protected AppApi
        (
            AppKey appKey,
            IAppApiUser user,
            ResourceAccess access
        )
        {
            AppKey = appKey;
            Path = new XtiPath(appKey.Name.DisplayText);
            this.user = user;
            Access = access ?? ResourceAccess.AllowAuthenticated();
        }

        private readonly IAppApiUser user;
        private readonly Dictionary<string, AppApiGroup> groups = new Dictionary<string, AppApiGroup>();

        public XtiPath Path { get; }

        public AppKey AppKey { get; }

        public ResourceAccess Access { get; }

        public Task<bool> HasAccess() => user.HasAccessToApp();

        protected TGroup AddGroup<TGroup>(Func<IAppApiUser, TGroup> createGroup)
            where TGroup : AppApiGroup
        {
            var group = createGroup(user);
            groups.Add(group.GroupName.ToLower(), group);
            return group;
        }

        public IEnumerable<AppApiGroup> Groups() => groups.Values.ToArray();

        public AppApiGroup Group(string groupName) => groups[groupName.ToLower()];

        public AppApiTemplate Template() => new AppApiTemplate(this);

        internal IEnumerable<AppRoleName> RoleNames()
        {
            var roleNames = new List<AppRoleName>();
            roleNames.AddRange(Access.Allowed);
            roleNames.AddRange(Access.Denied);
            var groupRoleNames = groups
                .Values
                .SelectMany(g => g.RoleNames())
                .Distinct();
            roleNames.AddRange(groupRoleNames);
            return roleNames.Distinct();
        }

        public override string ToString() => $"{GetType().Name} {Path.App}";
    }
}
