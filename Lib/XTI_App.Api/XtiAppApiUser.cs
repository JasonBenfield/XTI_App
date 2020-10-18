using System.Linq;
using System.Threading.Tasks;

namespace XTI_App.Api
{
    public sealed class XtiAppApiUser : IAppApiUser
    {
        private readonly IAppContext appContext;
        private readonly IUserContext userContext;

        public XtiAppApiUser(IAppContext appContext, IUserContext userContext)
        {
            this.appContext = appContext;
            this.userContext = userContext;
        }

        public async Task<bool> HasAccessToApp()
        {
            var app = await appContext.App();
            var user = await userContext.User();
            var userRoles = await user.RolesForApp(app);
            return userRoles.Any();
        }

        public async Task<bool> HasAccess(ResourceAccess resourceAccess, AccessModifier modifier)
        {
            var app = await appContext.App();
            var roles = await app.Roles();
            var allowedRoles = resourceAccess.Allowed.Select(ar => roles.FirstOrDefault(r => r.Name().Equals(ar)));
            var user = await userContext.User();
            bool hasAccess = false;
            if (user.UserName().Equals(AppUserName.Anon))
            {
                hasAccess = resourceAccess.IsAnonymousAllowed;
            }
            else if (!resourceAccess.Allowed.Any() && !resourceAccess.Denied.Any())
            {
                hasAccess = true;
            }
            else
            {
                var userRoles = await user.RolesForApp(app);
                var defaultUserRoles = userRoles
                    .Where(ur => ur.Modifier().Equals(AccessModifier.Default));
                var modifiedUserRoles = modifier.Equals(AccessModifier.Default)
                    ? Enumerable.Empty<AppUserRole>()
                    : userRoles.Where(ur => ur.Modifier().Equals(modifier));
                if (defaultUserRoles.Any(ur => allowedRoles.Any(ar => ur.IsRole(ar))))
                {
                    hasAccess = true;
                }
                if (!hasAccess && modifiedUserRoles.Any(ur => allowedRoles.Any(ar => ur.IsRole(ar))))
                {
                    hasAccess = true;
                }
                var deniedRoles = resourceAccess.Denied.Select
                (
                    dr => roles.FirstOrDefault(r => r.Name().Equals(dr))
                );
                if (defaultUserRoles.Any(ur => deniedRoles.Any(ar => ur.IsRole(ar))))
                {
                    hasAccess = false;
                }
                if (modifiedUserRoles.Any(ur => deniedRoles.Any(dr => ur.IsRole(dr))))
                {
                    hasAccess = false;
                }
            }
            return hasAccess;
        }
    }
}
