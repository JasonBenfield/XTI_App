using System.Collections.Generic;
using System.Linq;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public sealed class ResourceAccess
    {
        public static ResourceAccess AllowAnonymous()
            => new ResourceAccess(new AppRoleName[] { }, new AppRoleName[] { }, true);

        public static ResourceAccess AllowAuthenticated()
            => new ResourceAccess(new AppRoleName[] { }, new AppRoleName[] { }, false);

        public ResourceAccess(IEnumerable<AppRoleName> allowed, IEnumerable<AppRoleName> denied)
            : this(allowed, denied, false)
        {
        }

        private ResourceAccess(IEnumerable<AppRoleName> allowed, IEnumerable<AppRoleName> denied, bool isAnonAllowed)
        {
            Allowed = (allowed ?? new AppRoleName[] { });
            Denied = (denied ?? new AppRoleName[] { });
            IsAnonymousAllowed = isAnonAllowed;
        }

        public IEnumerable<AppRoleName> Allowed { get; }
        public IEnumerable<AppRoleName> Denied { get; }
        public bool IsAnonymousAllowed { get; }

        public ResourceAccess WithAllowed(params AppRoleName[] allowed)
        {
            return new ResourceAccess(Allowed.Union(allowed ?? new AppRoleName[] { }), Denied.ToArray());
        }

        public ResourceAccess WithDenied(params AppRoleName[] denied)
        {
            return new ResourceAccess(Allowed.ToArray(), Denied.Union(denied ?? new AppRoleName[] { }));
        }

        public override string ToString()
        {
            var allowed = string.Join(",", Allowed.Select(r => r.DisplayText));
            var denied = string.Join(",", Denied.Select(r => r.DisplayText));
            return $"{nameof(ResourceAccess)}\r\nAllowed: {allowed}\r\nDenied: {denied}";
        }
    }
}
