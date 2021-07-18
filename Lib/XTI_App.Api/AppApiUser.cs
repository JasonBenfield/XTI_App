using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public sealed class AppApiUser : IAppApiUser
    {
        private readonly IAppContext appContext;
        private readonly IUserContext userContext;
        private readonly IXtiPathAccessor pathAccessor;

        public AppApiUser(IAppContext appContext, IUserContext userContext, IXtiPathAccessor pathAccessor)
        {
            this.appContext = appContext;
            this.userContext = userContext;
            this.pathAccessor = pathAccessor;
        }

        public async Task<bool> HasAccess(ResourceAccess resourceAccess)
        {
            var app = await appContext.App();
            var roles = await app.Roles();
            var allowedRoles = resourceAccess.Allowed
                .Select(ar => roles.FirstOrDefault(r => r.Name().Equals(ar)));
            var user = await userContext.User();
            bool hasAccess = false;
            if (user.UserName().Equals(AppUserName.Anon))
            {
                hasAccess = resourceAccess.IsAnonymousAllowed;
            }
            else if (!resourceAccess.Allowed.Any())
            {
                hasAccess = true;
            }
            else
            {
                var path = pathAccessor.Value();
                var version = await app.Version(path.Version);
                var group = await version.ResourceGroup(path.Group);
                var modCategory = await group.ModCategory();
                var modifier = await modCategory.Modifier(path.Modifier);
                if (modifier.ModKey().Equals(path.Modifier))
                {
                    var userRoles = await user.Roles(app, modifier);
                    if (userRoles.Any(ur => allowedRoles.Any(ar => ur.ID.Equals(ar.ID))))
                    {
                        hasAccess = true;
                    }
                }
            }
            return hasAccess;
        }
    }
}
