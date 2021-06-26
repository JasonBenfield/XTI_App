using System.Collections.Generic;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public class AppApiWrapper : IAppApi
    {
        protected readonly AppApi source;

        public AppApiWrapper(AppApi source)
        {
            this.source = source;
        }

        public XtiPath Path { get => source.Path; }
        public AppKey AppKey { get => source.AppKey; }
        public ResourceAccess Access { get => source.Access; }
        public IAppApiGroup Group(string groupName) => source.Group(groupName);
        public IEnumerable<IAppApiGroup> Groups() => source.Groups();
        public AppApiTemplate Template() => source.Template();
    }
}
