using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace XTI_App.DB
{
    public sealed class AppDbReset
    {
        private readonly AppDbContext appDbContext;

        public AppDbReset(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task Run()
        {
            await appDbContext.Database.ExecuteSqlRawAsync
            (
                @"
exec sp_MSForEachTable 'IF OBJECT_ID(''?'') <> ISNULL(OBJECT_ID(''[dbo].[__EFMigrationsHistory]''),0) ALTER TABLE ? NOCHECK CONSTRAINT all';

exec sp_MSForEachTable '
    set rowcount 0; 
    SET QUOTED_IDENTIFIER ON; 
    IF OBJECT_ID(''?'') <> ISNULL(OBJECT_ID(''[dbo].[__EFMigrationsHistory]''),0) 
        DELETE FROM ?;';

exec sp_MSForEachTable 'IF OBJECT_ID(''?'') <> ISNULL(OBJECT_ID(''[dbo].[__EFMigrationsHistory]''),0) ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all';

exec sp_MSForEachTable 'IF OBJECT_ID(''?'') <> ISNULL(OBJECT_ID(''[dbo].[__EFMigrationsHistory]''),0) DBCC CHECKIDENT(''?'', RESEED, 0)';
"
            );
        }
    }
}
