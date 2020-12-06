using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppRequestRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppRequestRecord> repo;

        internal AppRequestRepository(AppFactory factory, DataRepository<AppRequestRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal async Task<AppRequest> Add(AppSession session, string requestKey, IAppVersion version, IResource resource, Modifier modifier, string path, DateTime timeRequested)
        {
            var record = new AppRequestRecord
            {
                SessionID = session.ID.Value,
                RequestKey = requestKey,
                VersionID = version.ID.Value,
                ResourceID = resource.ID.Value,
                ModifierID = modifier.ID.Value,
                Path = path ?? "",
                TimeStarted = timeRequested
            };
            await repo.Create(record);
            return factory.Request(record);
        }

        public async Task<AppRequest> Request(string requestKey)
        {
            var requestRecord = await repo.Retrieve()
                .FirstOrDefaultAsync(r => r.RequestKey == requestKey);
            return factory.Request(requestRecord);
        }

        internal async Task<AppRequest> AddDefaultRequest(AppSession session, DateTime timeRequested)
        {
            var app = await factory.Apps().App(AppKey.Unknown);
            var version = await app.CurrentVersion();
            var resourceGroup = await app.ResourceGroup(ResourceGroupName.Unknown);
            var resource = await resourceGroup.Resource(ResourceName.Unknown);
            var modCategory = await factory.ModCategories().Category(app, ModifierCategoryName.Default);
            var modifier = await modCategory.Modifier(ModifierKey.Default);
            var record = new AppRequestRecord
            {
                SessionID = session.ID.Value,
                RequestKey = new GeneratedKey().Value(),
                VersionID = version.ID.Value,
                ResourceID = resource.ID.Value,
                ModifierID = modifier.ID.Value,
                Path = "",
                TimeStarted = timeRequested,
                TimeEnded = timeRequested
            };
            await repo.Create(record);
            return factory.Request(record);
        }

        internal async Task<IEnumerable<AppRequest>> RetrieveBySession(AppSession session)
        {
            var requests = await repo.Retrieve()
                .Where(r => r.SessionID == session.ID.Value)
                .ToArrayAsync();
            return requests.Select(r => factory.Request(r));
        }
    }
}
