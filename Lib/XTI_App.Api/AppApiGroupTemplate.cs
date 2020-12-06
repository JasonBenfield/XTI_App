using System;
using System.Collections.Generic;
using System.Linq;

namespace XTI_App.Api
{
    public sealed class AppApiGroupTemplate
    {
        public AppApiGroupTemplate(string name, ModifierCategoryName modCategory, ResourceAccess access, IEnumerable<AppApiActionTemplate> actionTemplates)
        {
            Name = name;
            ModCategory = modCategory;
            Access = access;
            ActionTemplates = actionTemplates;
        }

        public string Name { get; }
        public ModifierCategoryName ModCategory { get; set; }
        public bool HasModifier { get => !ModCategory.Equals(ModifierCategoryName.Default); }
        public ResourceAccess Access { get; }
        public IEnumerable<AppApiActionTemplate> ActionTemplates { get; }

        public IEnumerable<ObjectValueTemplate> ObjectTemplates() =>
            ActionTemplates
                .SelectMany(a => a.ObjectTemplates())
                .Distinct();

        public bool IsUser() => Name.Equals("User", StringComparison.OrdinalIgnoreCase);
    }
}
