﻿using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Api;

public interface IAppApiAction
{
    string ActionName { get; }
    XtiPath Path { get; }
    string FriendlyName { get; }
    ResourceAccess Access { get; }
    ScheduledAppAgendaItemOptions Schedule { get; }
    RequestDataLoggingTypes RequestDataLoggingType { get; }
    bool IsResultDataLoggingEnabled { get; }

    ThrottledLogPath ThrottledLogPath(XtiBasePath xtiBasePath);

    AppApiActionTemplate Template();
}