using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.SyncFunction
{
    public class SyncFunction
    {
        private ILogger<SyncFunction> _logger;
        private IMapper _mapper;
        private AzureSqlDbContext _sql;
        private CosmosDbContext _cosmos;

        public SyncFunction(
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

        [Function("SyncFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // SetupEnvironment();
            return new OkObjectResult("ok");
        }

        // [Function("SyncFunction")]
        // public async Task<IActionResult> Run([TimerTrigger("0 */30 * * * *")] TimerInfo timerInfo)
        // {
        //     _logger.LogInformation("Start sync");
        //     return await SyncProjects();
        // }

        public async Task<IActionResult> SyncProjects()
        {
            // if (!_sql.UnsyncronizedProjects.Any())
            // {
            //     return new OkObjectResult("There is no projects to sync");
            // }
            //
            // List<System.Threading.Tasks.Task> saveChangesResults = new List<System.Threading.Tasks.Task>();
            // using (var sqlTransaction = _sql.Database.BeginTransaction())
            // {
            //     _logger.LogInformation("");
            //     _logger.LogInformation("Get All unsync projects from SQL");
            //     var sqlUnsyncProjects = await _sql.UnsyncronizedProjects
            //         .Include(p => p.Tasks)
            //         .ToListAsync();
            //
            //     var cosmosProjects = _mapper.Map<List<Model.CosmosDb.Project>>(sqlUnsyncProjects);
            //     foreach (var project in cosmosProjects)
            //     {
            //         project.PartitionKey = "Test";
            //         foreach (var task in project.Tasks)
            //         {
            //             task.PartitionKey = "Test";
            //         }
            //     }
            //     var cosmosTasks = cosmosProjects
            //         .SelectMany(p => p.Tasks);
            //     _logger.LogInformation("");
            //     _logger.LogInformation("Add Projects to CosmosDb");
            //     await _cosmos.Projects.AddRangeAsync(cosmosProjects);
            //     await _cosmos.Tasks.AddRangeAsync(cosmosTasks);
            //     saveChangesResults.Add(_cosmos.SaveChangesAsync());
            //
            //     _logger.LogInformation("");
            //     _logger.LogInformation("Remove projects from unsyncronized table");
            //     _sql.UnsyncronizedProjects.RemoveRange(sqlUnsyncProjects);
            //
            //     saveChangesResults.Add(_sql.SaveChangesAsync());
            //     await System.Threading.Tasks.Task.WhenAll(saveChangesResults);
            //     sqlTransaction.Commit();
            //     return new OkObjectResult("Data was synced successfully");
            // }
            return new OkResult();
        }
    }
}
