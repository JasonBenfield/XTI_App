using System.Collections.Generic;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public interface IAppApi
    {
        XtiPath Path { get; }
        AppKey AppKey { get; }
        ResourceAccess Access { get; }
        IEnumerable<IAppApiGroup> Groups();
        IAppApiGroup Group(string groupName);
        AppApiTemplate Template();
    }
}
