using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.SyncFunction.First
{
    public class TicketsSynchronizer : Synchronizer<DataAccess.AzureSQL.Ticket, DataAccess.CosmosDb.Ticket>
    {
        public TicketsSynchronizer(
            ILogger<Synchronizer<DataAccess.AzureSQL.Ticket, DataAccess.CosmosDb.Ticket>> logger,
            IMapper mapper,
            AzureSqlDbContext sql,
            CosmosDbContext cosmos
        ) : base(logger, mapper, sql, cosmos) { }

        public override async Task<bool> SynchronizeAsync()
        {
            return await SynchronizeEntities("tickets", _sql.Tickets, _cosmos.Tickets);
        }
    }
}