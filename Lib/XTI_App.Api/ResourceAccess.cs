using System.Collections.Generic;
using System.Linq;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public sealed class ResourceAccess
    {
        public static ResourceAccess AllowAnonymous()
            => new ResourceAccess(new AppRoleName[] { }, true);

        public static ResourceAccess AllowAuthenticated()
            => new ResourceAccess(new AppRoleName[] { }, false);

        public ResourceAccess(IEnumerable<AppRoleName> allowed)
            : this(allowed, false)
        {
        }

        private ResourceAccess(IEnumerable<AppRoleName> allowed, bool isAnonAllowed)
        {
            Allowed = (allowed ?? new AppRoleName[] { });
            IsAnonymousAllowed = isAnonAllowed;
        }

        public IEnumerable<AppRoleName> Allowed { get; }
        public bool IsAnonymousAllowed { get; }

        public ResourceAccess WithAllowed(params AppRoleName[] allowed)
            => new ResourceAccess(Allowed.Union(allowed ?? new AppRoleName[] { }));

        public ResourceAccess WithoutAllowed(params AppRoleName[] allowed)
            => new ResourceAccess(Allowed.Except(allowed ?? new AppRoleName[] { }));

        public override string ToString()
        {
            var allowed = string.Join(",", Allowed.Select(r => r.DisplayText));
            return $"{nameof(ResourceAccess)}\r\nAllowed: {allowed}";
        }
    }
}
