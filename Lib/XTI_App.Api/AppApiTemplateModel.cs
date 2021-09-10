using XTI_App.Abstractions;
using System.Linq;

namespace XTI_App.Api
{
    public sealed class AppApiTemplateModel
    {
        public AppKeyModel AppKey { get; set; } = new AppKeyModel();
        public AppApiGroupTemplateModel[] GroupTemplates { get; set; } = new AppApiGroupTemplateModel[] { };

        public string[] RecursiveRoles()
            => GroupTemplates.SelectMany(g => g.RecursiveRoles()).Distinct().ToArray();
    }
}
