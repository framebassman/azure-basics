using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.Sync.Model.Sql;
using ProjectTasks.Sync.Model.CosmosDb;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.Sync
{
    public class SyncFunction
    {
        private readonly ILogger<SyncFunction> _logger;
        private IMapper _mapper;
        private SqlContext _sql;
        private CosmosDbContext _cosmos;

        public SyncFunction(
            ILogger<SyncFunction> logger,
            IMapper mapper,
            SqlContext sql,
            CosmosDbContext cosmos
        )
        {
            _logger = logger;
            _mapper = mapper;
            _sql = sql;
            _cosmos = cosmos;
        }

        [Function("SyncFunction")]
        public async Task<IActionResult> Run([TimerTrigger("0 */30 * * * *")] TimerInfo timerInfo)
        {
            _logger.LogInformation("Start sync");
            return await SyncProjects();
        }

        public async Task<IActionResult> SyncProjects()
        {
            if (!_sql.UnsyncronizedProjects.Any())
            {
                return new OkObjectResult("There is no projects to sync");
            }

            List<System.Threading.Tasks.Task> saveChangesResults = new List<System.Threading.Tasks.Task>();
            using (var sqlTransaction = _sql.Database.BeginTransaction())
            {
                _logger.LogInformation("");
                _logger.LogInformation("Get All unsync projects from SQL");
                var sqlUnsyncProjects = await _sql.UnsyncronizedProjects
                    .Include(p => p.Tasks)
                    .ToListAsync();

                var cosmosProjects = _mapper.Map<List<Model.CosmosDb.Project>>(sqlUnsyncProjects);
                foreach (var project in cosmosProjects)
                {
                    project.PartitionKey = "Test";
                    foreach (var task in project.Tasks)
                    {
                        task.PartitionKey = "Test";
                    }
                }
                var cosmosTasks = cosmosProjects
                    .SelectMany(p => p.Tasks);
                _logger.LogInformation("");
                _logger.LogInformation("Add Projects to CosmosDb");
                await _cosmos.Projects.AddRangeAsync(cosmosProjects);
                await _cosmos.Tasks.AddRangeAsync(cosmosTasks);
                saveChangesResults.Add(_cosmos.SaveChangesAsync());

                _logger.LogInformation("");
                _logger.LogInformation("Move unsyncronized projects to projects in Sql");
                var sqlProjects = _mapper.Map<List<Model.Sql.Project>>(sqlUnsyncProjects);
                await _sql.Projects.AddRangeAsync(sqlProjects);
                _sql.UnsyncronizedProjects.RemoveRange(sqlUnsyncProjects);
                saveChangesResults.Add(_sql.SaveChangesAsync());

                var finishedTask = System.Threading.Tasks.Task.WhenAll(saveChangesResults);
                await finishedTask.ContinueWith(x => { });
                sqlTransaction.Commit();
                return new OkObjectResult("Data was synced successfully");
            }
        }
    }
}
