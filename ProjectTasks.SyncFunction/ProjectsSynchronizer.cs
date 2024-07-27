using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.SyncFunction
{
    public class ProjectsSynchronizer
    {
        private ILogger<SyncFunction> _logger;
        private IMapper _mapper;
        private AzureSqlDbContext _sql;
        private CosmosDbContext _cosmos;

        public ProjectsSynchronizer(
            ILogger<SyncFunction> logger,
            IMapper mapper,
            AzureSqlDbContext sql,
            CosmosDbContext cosmos
        )
        {
            _logger = logger;
            _mapper = mapper;
            _sql = sql;
            _cosmos = cosmos;
        }

        public async Task<bool> Synchronize()
        {
            if (await _sql.Projects.Where(project => !project.WasSynchronized).CountAsync() == 0)
            {
                _logger.LogInformation("There is no projects to sync");
                return true;
            }

            using (var sqlTransaction = _sql.Database.BeginTransaction())
            {
                var saveChangesResults = new List<Task>();
                _logger.LogInformation("Get all unsync projects with tickets from SQL");
                var sqlUnsyncProjects = await _sql.Projects
                    .Where(project => !project.WasSynchronized)
                    .ToListAsync();

                var cosmosProjects = _mapper.Map<List<DataAccess.CosmosDb.Project>>(sqlUnsyncProjects);
                cosmosProjects.ForEach(project => project.PartitionKey = "Test");
                await _cosmos.Projects.AddRangeAsync(cosmosProjects);
                saveChangesResults.Add(_cosmos.SaveChangesAsync());

                sqlUnsyncProjects.ForEach(project => project.WasSynchronized = true);
                saveChangesResults.Add(_sql.SaveChangesAsync());

                await Task.WhenAll(saveChangesResults);
                await sqlTransaction.CommitAsync();
                return true;
            }
        }
    }
}
