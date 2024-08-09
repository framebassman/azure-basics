using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.SyncFunction;

public class ProjectsSynchronizer(
    ILogger<ProjectsSynchronizer> logger,
    IMapper mapper,
    AzureSqlDataProvider sql,
    CosmosDbDataProvider cosmos)
    : ISynchronizer
{
    public async Task<bool> SynchronizeAsync(CancellationToken token)
    {
        var lastSyncId = await cosmos.GetLastSynchronizedProjectId(token);
        var sqlUnsync = await sql.GetProjectsToSync(project => project.Id > lastSyncId, token);

        if (sqlUnsync.Count == 0)
        {
            logger.LogInformation("There is no projects to sync");
            return false;
        }

        var cosmosSync = mapper.Map<List<DataAccess.CosmosDb.Project>>(sqlUnsync);
        await cosmos.AddProjectsBulk(cosmosSync, token);
        var lastProjectToSync = sqlUnsync.TakeLast(1).First();
        await cosmos.UpdateLastSynchronizedProjectId(lastProjectToSync.Id, token);
        logger.LogInformation($"{sqlUnsync.Count} projects were synchronized");

        return true;
    }
}
