using System.Collections.Generic;
using System.Linq;

namespace XTI_App
{
    public class AppRoleNames
    {
        public static readonly AppRoleNames Empty = new AppRoleNames();

        protected AppRoleNames()
        {
        }

        private readonly List<AppRoleName> roleNames = new List<AppRoleName>();

        protected AppRoleName Add(string value)
        {
            var roleName = new AppRoleName(value);
            roleNames.Add(roleName);
            return roleName;
        }

        public IEnumerable<AppRoleName> Values() => roleNames.ToArray();

        public override string ToString()
        {
            var joined = string.Join(", ", roleNames.Select(rn => rn.DisplayText));
            return $"{GetType().Name} {joined}";
        }
    }
}
