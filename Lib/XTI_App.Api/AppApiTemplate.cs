using System;
using System.Collections.Generic;
using System.Linq;

namespace XTI_App.Api
{
    public sealed class AppApiTemplate
    {
        public AppApiTemplate(AppApi api)
        {
            Name = api.Name.App;
            GroupTemplates = api.Groups().Select(g => g.Template());
        }

        public string Name { get; }
        public IEnumerable<AppApiGroupTemplate> GroupTemplates { get; }

        public IEnumerable<ObjectValueTemplate> ObjectTemplates() =>
            GroupTemplates
                .SelectMany(g => g.ObjectTemplates())
                .Distinct();

        public bool IsHub() => Name.Equals("Hub", StringComparison.OrdinalIgnoreCase);
    }
}
