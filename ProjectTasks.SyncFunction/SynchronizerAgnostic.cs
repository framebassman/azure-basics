using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.SyncFunction;

public class SynchronizerAgnostic
{
    private ILogger<SynchronizerAgnostic> _logger;
    private IMapper _mapper;
    protected AzureSqlDataProvider _sql;
    protected CosmosDbDataProvider _cosmos;

    public SynchronizerAgnostic(
        ILogger<SynchronizerAgnostic> logger,
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

    public async Task<bool> SynchronizeProjects(CancellationToken token)
    {
        var lastSyncId = await _sql.GetLastSynchronizedProjectId(token);
        var sqlUnsync = await _sql.GetProjectsToSync(project => project.Id > lastSyncId, token);

        if (sqlUnsync.Count == 0)
        {
            _logger.LogInformation($"There is no Projects to sync");
            return false;
        }

        var cosmosSync = _mapper.Map<List<DataAccess.CosmosDb.Project>>(sqlUnsync);
        await _cosmos.AddProjectsBulk(cosmosSync, token);
        var lastProjectToSync = sqlUnsync.TakeLast(1).First();
        await _sql.UpdateLastSynchronizedProjectId(lastProjectToSync.Id, token);
        _logger.LogInformation($"{sqlUnsync.Count} projects were synchronized");

        return true;
    }
}
