using System;
using System.Collections.Generic;
using System.Linq;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public sealed class AppApiGroupTemplate
    {
        public AppApiGroupTemplate(string name, ModifierCategoryName modCategory, ResourceAccess access, IEnumerable<AppApiActionTemplate> actionTemplates)
        {
            Name = name.Replace(" ", "");
            ModCategory = modCategory;
            Access = access;
            ActionTemplates = actionTemplates;
        }

        public string Name { get; }
        public ModifierCategoryName ModCategory { get; set; }
        public bool HasModifier { get => !ModCategory.Equals(ModifierCategoryName.Default); }
        public ResourceAccess Access { get; }
        public IEnumerable<AppApiActionTemplate> ActionTemplates { get; }

        public IEnumerable<FormValueTemplate> FormTemplates() =>
            ActionTemplates
                .SelectMany(a => a.FormTemplates())
                .Distinct();

        public IEnumerable<ObjectValueTemplate> ObjectTemplates() =>
            ActionTemplates
                .SelectMany(a => a.ObjectTemplates())
                .Distinct();

        public IEnumerable<NumericValueTemplate> NumericValueTemplates() =>
            ActionTemplates
                .SelectMany(a => a.NumericValueTemplates())
                .Distinct();

        public bool IsUser() => Name.Equals("User", StringComparison.OrdinalIgnoreCase);
    }
}
