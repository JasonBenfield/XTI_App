using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace XTI_Core.EF
{
    public sealed class UnitOfWork
    {
        private readonly DbContext dbContext;
        private readonly bool isInMemory;

        public UnitOfWork(DbContext dbContext)
        {
            this.dbContext = dbContext;
            isInMemory = dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        }

        public async Task BeginTransaction()
        {
            if (!isInMemory)
            {
                await dbContext.Database.BeginTransactionAsync();
            }
        }

        public async Task Commit()
        {
            if (!isInMemory)
            {
                await dbContext.Database.CurrentTransaction.CommitAsync();
            }
        }

        public async Task Rollback()
        {
            if (!isInMemory)
            {
                await dbContext.Database.CurrentTransaction.RollbackAsync();
            }
        }

        public bool IsInProgress() => dbContext.Database.CurrentTransaction != null;

        public async Task Execute(Func<Task> action)
        {
            if (IsInProgress())
            {
                await action();
            }
            else
            {
                await BeginTransaction();
                try
                {
                    await action();
                    await Commit();
                }
                catch
                {
                    await Rollback();
                    throw;
                }
            }
        }
    }
}
