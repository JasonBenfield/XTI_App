using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Api;

public interface IAppApi
{
    XtiPath Path { get; }
    AppKey AppKey { get; }
    ResourceAccess Access { get; }
    IAppApiGroup[] Groups();
    IAppApiGroup Group(string groupName);
    AppApiTemplate Template();
    bool HasAction(XtiPath xtiPath);
    IAppApiAction GetAction(XtiPath xtiPath);

    ThrottledLogPath[] ThrottledLogPaths(XtiBasePath xtiBasePath);
    ScheduledAppAgendaItemOptions[] ActionSchedules();
}