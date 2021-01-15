using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppRequestRepository
    {
        private readonly IMainDataRepositoryFactory repoFactory;
        private readonly AppFactory factory;
        private readonly DataRepository<AppRequestRecord> repo;

        internal AppRequestRepository(IMainDataRepositoryFactory repoFactory, AppFactory factory)
        {
            this.repoFactory = repoFactory;
            this.factory = factory;
            repo = repoFactory.CreateRequests();
        }

        internal async Task<AppRequest> Add(AppSession session, string requestKey, AppVersion version, Resource resource, Modifier modifier, string path, DateTimeOffset timeRequested)
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

        internal async Task<IEnumerable<AppRequest>> RetrieveBySession(AppSession session)
        {
            var requests = await repo.Retrieve()
                .Where(r => r.SessionID == session.ID.Value)
                .ToArrayAsync();
            return requests.Select(r => factory.Request(r));
        }

        internal async Task<IEnumerable<AppRequest>> RetrieveMostRecent(AppSession session, int howMany)
        {
            var requests = await repo.Retrieve()
                .Where(r => r.SessionID == session.ID.Value)
                .OrderByDescending(r => r.TimeStarted)
                .Take(howMany)
                .ToArrayAsync();
            return requests.Select(r => factory.Request(r));
        }

        internal async Task<IEnumerable<AppRequestExpandedModel>> MostRecentForApp(App app, int howMany)
        {
            var resourceGroupIDs = repoFactory
                .CreateResourceGroups()
                .Retrieve()
                .Where(rg => rg.AppID == app.ID.Value)
                .Select(rg => rg.ID);
            var resourceIDs = repoFactory
                .CreateResources()
                .Retrieve()
                .Where(r => resourceGroupIDs.Any(rg => rg == r.GroupID))
                .Select(r => r.ID);
            var resources = repoFactory
                .CreateResources()
                .Retrieve()
                .Join
                (
                    repoFactory
                        .CreateResourceGroups()
                        .Retrieve()
                        .Where(rg => rg.AppID == app.ID.Value),
                    res => res.GroupID,
                    rg => rg.ID,
                    (res, rg) => new
                    {
                        ResourceID = res.ID,
                        ActionName = res.Name,
                        GroupID = rg.ID,
                        GroupName = rg.Name
                    }
                );
            var requests = await repo.Retrieve()
                .Join
                (
                    resources,
                    req => req.ResourceID,
                    res => res.ResourceID,
                    (req, res) => new
                    {
                        ID = req.ID,
                        SessionID = req.SessionID,
                        GroupName = res.GroupName,
                        ActionName = res.ActionName,
                        TimeStarted = req.TimeStarted,
                        TimeEnded = req.TimeEnded
                    }
                )
                .Join
                (
                    repoFactory
                        .CreateSessions()
                        .Retrieve()
                        .Join
                        (
                            repoFactory
                                .CreateUsers()
                                .Retrieve(),
                            s => s.UserID,
                            u => u.ID,
                            (s, u) => new
                            {
                                SessionID = s.ID,
                                u.UserName
                            }
                        ),
                    req => req.SessionID,
                    s => s.SessionID,
                    (req, s) => new
                    {
                        ID = req.ID,
                        UserName = s.UserName,
                        GroupName = req.GroupName,
                        ActionName = req.ActionName,
                        TimeStarted = req.TimeStarted,
                        TimeEnded = req.TimeEnded
                    }
                )
                .OrderByDescending(r => r.TimeStarted)
                .Take(howMany)
                .Select
                (
                    r => new AppRequestExpandedModel
                    {
                        ID = r.ID,
                        UserName = r.UserName,
                        GroupName = new ResourceGroupName(r.GroupName).DisplayText,
                        ActionName = new ResourceName(r.ActionName).DisplayText,
                        TimeStarted = r.TimeStarted,
                        TimeEnded = r.TimeEnded
                    }
                )
                .ToArrayAsync();
            return requests;
        }
    }
}
