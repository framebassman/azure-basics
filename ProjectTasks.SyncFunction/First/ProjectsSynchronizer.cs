using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.SyncFunction.First
{
    public class ProjectsSynchronizer : Synchronizer<DataAccess.AzureSQL.Project, DataAccess.CosmosDb.Project>
    {
        public ProjectsSynchronizer(
            ILogger<Synchronizer<DataAccess.AzureSQL.Project, DataAccess.CosmosDb.Project>> logger,
            IMapper mapper,
            AzureSqlDbContext sql,
            CosmosDbContext cosmos
        ) : base(logger, mapper, sql, cosmos) { }

        public override async Task<bool> SynchronizeAsync()
        {
            return await SynchronizeEntities("projects", _sql.Projects, _cosmos.Projects);
        }
    }
}
