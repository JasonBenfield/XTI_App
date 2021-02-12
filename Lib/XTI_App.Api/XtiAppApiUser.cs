using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public sealed class XtiAppApiUser : IAppApiUser
    {
        private readonly IAppContext appContext;
        private readonly IUserContext userContext;
        private readonly XtiPath path;

        public XtiAppApiUser(IAppContext appContext, IUserContext userContext, XtiPath path)
        {
            this.appContext = appContext;
            this.userContext = userContext;
            this.path = path;
        }

        public async Task<bool> HasAccessToApp()
        {
            var app = await appContext.App();
            var user = await userContext.User();
            var userRoles = await user.Roles(app);
            return userRoles.Any();
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
            else if (!resourceAccess.Allowed.Any() && !resourceAccess.Denied.Any())
            {
                hasAccess = true;
            }
            else
            {
                var userRoles = await user.Roles(app);
                if (userRoles.Any(ur => allowedRoles.Any(ar => ur.ID.Equals(ar.ID))))
                {
                    hasAccess = true;
                }
                var deniedRoles = resourceAccess.Denied.Select
                (
                    dr => roles.FirstOrDefault(r => r.Name().Equals(dr))
                );
                if (userRoles.Any(ur => deniedRoles.Any(dr => ur.ID.Equals(dr.ID))))
                {
                    hasAccess = false;
                }
            }
            if (hasAccess && !path.Modifier.Equals(ModifierKey.Default))
            {
                var version = await app.Version(path.Version);
                var group = await version.ResourceGroup(path.Group);
                var modCategory = await group.ModCategory();
                if (!modCategory.Name().Equals(ModifierCategoryName.Default))
                {
                    var userHasFullAccess = await user.IsModCategoryAdmin(modCategory);
                    if (userHasFullAccess)
                    {
                        hasAccess = true;
                    }
                    else
                    {
                        hasAccess = await user.HasModifier(path.Modifier);
                    }
                }
            }
            return hasAccess;
        }
    }
}
