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
            AppVersionKey versionKey,
            IAppApiUser user,
            ResourceAccess access
        )
        {
            AppKey = appKey;
            Path = new XtiPath(appKey.Name.DisplayText, versionKey.DisplayText);
            this.user = user;
            Access = access ?? ResourceAccess.AllowAuthenticated();
        }

        private readonly IAppApiUser user;
        private readonly Dictionary<string, AppApiGroup> groups = new Dictionary<string, AppApiGroup>();

        public XtiPath Path { get; }

        public AppKey AppKey { get; }

        public ResourceAccess Access { get; }

        public Task<bool> HasAccess() => user.HasAccessToApp(Path);

        protected TGroup AddGroup<TGroup>(Func<IAppApiUser, TGroup> createGroup)
            where TGroup : AppApiGroup
        {
            var group = createGroup(user);
            groups.Add(group.Path.Group.Value, group);
            return group;
        }

        public IEnumerable<AppApiGroup> Groups() => groups.Values.ToArray();

        public AppApiGroup Group(string groupName) => groups[groupName.ToLower()];

        public AppApiTemplate Template() => new AppApiTemplate(this);

        public override string ToString() => $"{GetType().Name} {Path.App}";
    }
}
