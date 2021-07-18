using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppEventRepository
    {
        private readonly AppFactory factory;

        public AppEventRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        public async Task<AppEvent> LogEvent(AppRequest request, string eventKey, DateTimeOffset timeOccurred, AppEventSeverity severity, string caption, string message, string detail)
        {
            var record = new AppEventRecord
            {
                RequestID = request.ID.Value,
                EventKey = eventKey,
                TimeOccurred = timeOccurred,
                Severity = severity.Value,
                Caption = caption,
                Message = message,
                Detail = detail
            };
            await factory.DB.Events.Create(record);
            return factory.Event(record);
        }

        internal Task<AppEvent[]> RetrieveByRequest(AppRequest request)
        {
            return factory.DB.Events.Retrieve()
                .Where(e => e.RequestID == request.ID.Value)
                .Select(e => factory.Event(e))
                .ToArrayAsync();
        }

        internal Task<AppEvent[]> MostRecentErrorsForVersion(AppVersion version, int howMany)
        {
            var requestIDs = factory.DB
                .Requests
                .Retrieve()
                .Join
                (
                    factory.DB.Resources
                        .Retrieve(),
                    req => req.ResourceID,
                    res => res.ID,
                    (req, res) => new { RequestID = req.ID, GroupID = res.GroupID }
                )
                .Join
                (
                    factory.DB.ResourceGroups
                        .Retrieve(),
                    res => res.GroupID,
                    rg => rg.ID,
                    (res, rg) => new { RequestID = res.RequestID, VersionID = rg.VersionID }
                )
                .Where(rg => rg.VersionID == version.ID.Value)
                .Select(rg => rg.RequestID);
            return mostRecentErrors(howMany, requestIDs);
        }

        internal Task<AppEvent[]> MostRecentErrorsForResourceGroup(ResourceGroup group, int howMany)
        {
            var requestIDs = factory.DB
                .Requests
                .Retrieve()
                .Join
                (
                    factory.DB
                        .Resources
                        .Retrieve(),
                    req => req.ResourceID,
                    res => res.ID,
                    (req, res) => new { RequestID = req.ID, GroupID = res.GroupID }
                )
                .Where(rg => rg.GroupID == group.ID.Value)
                .Select(rg => rg.RequestID);
            return mostRecentErrors(howMany, requestIDs);
        }

        internal Task<AppEvent[]> MostRecentErrorsForResource(Resource resource, int howMany)
        {
            var requestIDs = factory.DB
                .Requests
                .Retrieve()
                .Where(r => r.ResourceID == resource.ID.Value)
                .Select(r => r.ResourceID);
            return mostRecentErrors(howMany, requestIDs);
        }

        private Task<AppEvent[]> mostRecentErrors(int howMany, IQueryable<int> requestIDs)
            => factory.DB
                .Events
                .Retrieve()
                .Where
                (
                    evt => evt.Severity >= AppEventSeverity.Values.ValidationFailed.Value
                        && requestIDs.Any(id => evt.RequestID == id)
                )
                .OrderByDescending(evt => evt.TimeOccurred)
                .Take(howMany)
                .Select(evt => factory.Event(evt))
                .ToArrayAsync();

    }
}
