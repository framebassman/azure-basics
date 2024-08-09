using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.SyncFunction;

public class TicketsSynchronizer : ISynchronizer
{
    private ILogger<TicketsSynchronizer> _logger;
    private IMapper _mapper;
    protected AzureSqlDataProvider _sql;
    protected CosmosDbDataProvider _cosmos;

    public TicketsSynchronizer(
        ILogger<TicketsSynchronizer> logger,
        IMapper mapper,
        AzureSqlDataProvider sql,
        CosmosDbDataProvider cosmos
    )
    {
        _logger = logger;
        _mapper = mapper;
        _sql = sql;
        _cosmos = cosmos;
    }

    public async Task<bool> SynchronizeAsync(CancellationToken token)
    {
        var lastSyncId = await _cosmos.GetLastSynchronizedTicketId(token);
        var sqlUnsync = await _sql.GetTicketsToSync(ticket => ticket.Id > lastSyncId, token);

        if (sqlUnsync.Count == 0)
        {
            _logger.LogInformation("There is no tickets to sync");
            return false;
        }

        var cosmosSync = _mapper.Map<List<DataAccess.CosmosDb.Ticket>>(sqlUnsync);
        await _cosmos.AddTicketsBulk(cosmosSync, token);
        var lastProjectToSync = sqlUnsync.TakeLast(1).First();
        await _cosmos.UpdateLastSynchronizedTicketId(lastProjectToSync.Id, token);
        _logger.LogInformation($"{sqlUnsync.Count} tickets were synchronized");

        return true;
    }
}
