﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTI_App.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class ResourceGroupRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<ResourceGroupRecord> repo;

        public ResourceGroupRepository(AppFactory factory, DataRepository<ResourceGroupRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal async Task<ResourceGroup> Add(App app, ResourceGroupName name)
        {
            var record = new ResourceGroupRecord
            {
                AppID = app.ID.Value,
                Name = name.Value
            };
            await repo.Create(record);
            return factory.Group(record);
        }

        internal async Task<IEnumerable<ResourceGroup>> Groups(App app)
        {
            var records = await repo.Retrieve()
                .Where(g => g.AppID == app.ID.Value)
                .ToArrayAsync();
            return records.Select(g => factory.Group(g));
        }

        internal async Task<ResourceGroup> Group(App app, ResourceGroupName name)
        {
            var record = await repo.Retrieve()
                .Where(g => g.AppID == app.ID.Value && g.Name == name.Value)
                .FirstOrDefaultAsync();
            if (record == null)
            {
                record = await repo.Retrieve()
                    .Where(g => g.Name == ResourceGroupName.Unknown.Value)
                    .FirstOrDefaultAsync();
            }
            return factory.Group(record);
        }
    }
}