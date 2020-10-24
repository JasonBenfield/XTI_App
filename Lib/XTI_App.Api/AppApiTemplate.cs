using System;
using System.Collections.Generic;
using System.Linq;

namespace XTI_App.Api
{
    public sealed class AppApiTemplate
    {
        public AppApiTemplate(AppApi api)
        {
            AppKey = api.AppKey;
            GroupTemplates = api.Groups().Select(g => g.Template());
        }

        public AppKey AppKey { get; }
        public string Name { get => AppKey.Name; }
        public IEnumerable<AppApiGroupTemplate> GroupTemplates { get; }

        public IEnumerable<ObjectValueTemplate> ObjectTemplates() =>
            GroupTemplates
                .SelectMany(g => g.ObjectTemplates())
                .Distinct();

        public bool IsAuthenticator() => Name.Equals("Authenticator", StringComparison.OrdinalIgnoreCase);
    }
}
