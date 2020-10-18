using System;
using System.Collections.Generic;
using System.Linq;

namespace XTI_App.Api
{
    public sealed class AppApiGroupTemplate
    {
        public AppApiGroupTemplate(string name, bool hasModifier, ResourceAccess access, IEnumerable<AppApiActionTemplate> actionTemplates)
        {
            Name = name;
            HasModifier = hasModifier;
            Access = access;
            ActionTemplates = actionTemplates;
        }

        public string Name { get; }
        public bool HasModifier { get; }
        public ResourceAccess Access { get; }
        public IEnumerable<AppApiActionTemplate> ActionTemplates { get; }

        public IEnumerable<ObjectValueTemplate> ObjectTemplates() =>
            ActionTemplates
                .SelectMany(a => a.ObjectTemplates())
                .Distinct();

        public bool IsUser() => Name.Equals("User", StringComparison.OrdinalIgnoreCase);
    }
}
