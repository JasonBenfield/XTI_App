using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public sealed class AppApiActionTemplateModel
    {
        public string Name { get; set; }
        public bool IsAnonymousAllowed { get; set; }
        public string[] Roles { get; set; }
        public ResourceResultType ResultType { get; set; }
    }
}
