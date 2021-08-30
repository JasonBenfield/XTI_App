using System;
using System.Collections.Generic;
using System.Linq;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public sealed class AppApiTemplate
    {
        public AppApiTemplate(AppApi api)
        {
            AppKey = api.AppKey;
            GroupTemplates = api.Groups().Select(g => g.Template());
            RoleNames = api.RoleNames();
        }

        public AppKey AppKey { get; }
        public string Name { get => AppKey.Name.DisplayText.Replace(" ", ""); }
        public IEnumerable<AppRoleName> RoleNames { get; }
        public IEnumerable<AppApiGroupTemplate> GroupTemplates { get; }

        public IEnumerable<FormValueTemplate> FormTemplates() =>
            GroupTemplates
                .SelectMany(g => g.FormTemplates())
                .Distinct();

        public IEnumerable<ObjectValueTemplate> ObjectTemplates() =>
            GroupTemplates
                .SelectMany(g => g.ObjectTemplates())
                .Distinct();

        public IEnumerable<NumericValueTemplate> NumericValueTemplates() =>
            GroupTemplates
                .SelectMany(g => g.NumericValueTemplates())
                .Distinct();

        public bool IsAuthenticator() => Name.Equals("Hub", StringComparison.OrdinalIgnoreCase);
    }
}
