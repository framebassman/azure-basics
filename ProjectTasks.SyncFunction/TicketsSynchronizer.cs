using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.SyncFunction;

public class TicketsSynchronizer(
    ILogger<TicketsSynchronizer> logger,
    IMapper mapper,
    AzureSqlDataProvider sql,
    CosmosDbDataProvider cosmos)
    : ISynchronizer
{
    public async Task<bool> SynchronizeAsync(CancellationToken token)
    {
        var lastSyncId = await cosmos.GetLastSynchronizedTicketId(token);
        var sqlUnsync = await sql.GetTicketsToSync(ticket => ticket.Id > lastSyncId, token);

        if (sqlUnsync.Count == 0)
        {
            logger.LogInformation("There is no tickets to sync");
            return false;
        }

        var cosmosSync = mapper.Map<List<DataAccess.CosmosDb.Ticket>>(sqlUnsync);
        await cosmos.AddTicketsBulk(cosmosSync, token);
        var lastProjectToSync = sqlUnsync.TakeLast(1).First();
        await cosmos.UpdateLastSynchronizedTicketId(lastProjectToSync.Id, token);
        logger.LogInformation($"{sqlUnsync.Count} tickets were synchronized");

        return true;
    }
}
