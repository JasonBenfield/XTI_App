using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppEventRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppEventRecord> repo;

        public AppEventRepository(AppFactory factory, DataRepository<AppEventRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        public async Task<AppEvent> LogEvent(AppRequest request, string eventKey, DateTime timeOccurred, AppEventSeverity severity, string caption, string message, string detail)
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
            await repo.Create(record);
            return factory.Event(record);
        }

        internal async Task<IEnumerable<AppEvent>> RetrieveByRequest(AppRequest request)
        {
            var eventRepo = factory.Events();
            var records = await repo.Retrieve()
                .Where(e => e.RequestID == request.ID.Value)
                .ToArrayAsync();
            return records.Select(e => factory.Event(e));
        }
    }
}
