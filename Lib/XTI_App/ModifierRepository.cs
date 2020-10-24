﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class ModifierRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<ModifierRecord> repo;

        internal ModifierRepository(AppFactory factory, DataRepository<ModifierRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal async Task<Modifier> Add(ModifierCategory category, ModifierKey modKey, string targetID, string displayText)
        {
            var record = new ModifierRecord
            {
                CategoryID = category.ID.Value,
                ModKey = modKey.Value,
                TargetKey = targetID,
                DisplayText = displayText
            };
            await repo.Create(record);
            return factory.Modifier(record);
        }

        internal async Task<IEnumerable<Modifier>> Modifiers(ModifierCategory category)
        {
            var records = await repo.Retrieve()
                .Where(m => m.CategoryID == category.ID.Value)
                .ToArrayAsync();
            return records.Select(m => factory.Modifier(m));
        }

        internal async Task<Modifier> Modifier(ModifierKey modKey)
        {
            var record = await repo.Retrieve()
                .Where(m => m.ModKey == modKey.Value)
                .FirstOrDefaultAsync();
            if (record == null)
            {
                record = await repo.Retrieve()
                    .Where(m => m.ModKey == ModifierKey.Default.Value)
                    .FirstOrDefaultAsync();
            }
            return factory.Modifier(record);
        }

        internal async Task<Modifier> Modifier(ModifierCategory category, string targetKey)
        {
            var record = await repo.Retrieve()
                .Where(m => m.CategoryID == category.ID.Value && m.TargetKey == targetKey)
                .FirstOrDefaultAsync();
            return factory.Modifier(record);
        }
    }
}