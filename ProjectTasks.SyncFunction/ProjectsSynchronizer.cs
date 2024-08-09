using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.SyncFunction;

public class ProjectsSynchronizer : ISynchronizer
{
    private ILogger<ProjectsSynchronizer> _logger;
    private IMapper _mapper;
    protected AzureSqlDataProvider _sql;
    protected CosmosDbDataProvider _cosmos;

    public ProjectsSynchronizer(
        ILogger<ProjectsSynchronizer> logger,
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
        var lastSyncId = await _cosmos.GetLastSynchronizedProjectId(token);
        var sqlUnsync = await _sql.GetProjectsToSync(project => project.Id > lastSyncId, token);

        if (sqlUnsync.Count == 0)
        {
            _logger.LogInformation("There is no projects to sync");
            return false;
        }

        var cosmosSync = _mapper.Map<List<DataAccess.CosmosDb.Project>>(sqlUnsync);
        await _cosmos.AddProjectsBulk(cosmosSync, token);
        var lastProjectToSync = sqlUnsync.TakeLast(1).First();
        await _cosmos.UpdateLastSynchronizedProjectId(lastProjectToSync.Id, token);
        _logger.LogInformation($"{sqlUnsync.Count} projects were synchronized");

        return true;
    }
}
