using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.Sync.Model.Sql;
using ProjectTasks.Sync.Model.CosmosDb;
using AutoMapper;

namespace ProjectTasks.Sync
{
    public class MyHttpTrigger
    {
        private readonly ILogger<MyHttpTrigger> _logger;
        private IMapper _mapper;
        private SqlContext _sql;
        private CosmosDbContext _cosmos;

        public MyHttpTrigger(
            ILogger<MyHttpTrigger> logger,
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

        [Function("MyHttpTrigger")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            _cosmos.Projects.RemoveRange(_cosmos.Projects.ToList());
            _cosmos.Tasks.RemoveRange(_cosmos.Tasks.ToList());
            _cosmos.SaveChanges();
            _sql.Projects.RemoveRange(_sql.Projects.ToList());
            _sql.Tasks.RemoveRange(_sql.Tasks.ToList());
            _sql.SaveChanges();

            SyncProjects();
            return new OkObjectResult("Welcome to Azure Functions!");
        }

        public void SyncTasks()
        {
            // var sqlProjects = _sql.UnsyncronizedProjects.ToList();
            // _logger.LogInformation("{@sqlProjects}", sqlProjects);
            // var cosmosProjects = _cosmos.Projects.ToList();
            // _logger.LogInformation("{@cosmosProjects}", cosmosProjects);

            _logger.LogInformation("Get All unsync tasks");
            var sqlUnsyncronizedTasks = _sql.UnsyncronizedTasks.ToList();

            _logger.LogInformation("Map to CosmosDb Tasks");
            var cosmosTasks = _mapper.Map<List<Model.CosmosDb.Task>>(sqlUnsyncronizedTasks);
            foreach (var task in cosmosTasks)
            {
                task.PartitionKey = "Test";
            }
            _logger.LogInformation("{@cosmosTasks}", cosmosTasks);
            _logger.LogInformation("Add Tasks to CosmosDb");
            _cosmos.Tasks.AddRange(cosmosTasks);
            _cosmos.SaveChanges();

            _logger.LogInformation("Move unsyncronized tasks to tasks in Sql");
            var sqlTasks = _mapper.Map<List<Model.Sql.Task>>(sqlUnsyncronizedTasks);
            _sql.Tasks.AddRange(sqlTasks);
            _sql.SaveChanges();

            // read project from Unsyncronized projects
            // insert into CosmosDb
            // move from Unsyncronized to usual
        }

        public void SyncProjects()
        {
            _logger.LogInformation("Get All unsync projects");
            var sqlUnsyncProjects = _sql.UnsyncronizedProjects.ToList();

            _logger.LogInformation("Map to CosmosDb Projects");
            var cosmosProjects = _mapper.Map<List<Model.CosmosDb.Project>>(sqlUnsyncProjects);
            foreach (var project in cosmosProjects)
            {
                project.PartitionKey = "Test";
            }
            _logger.LogInformation("{@cosmosProjects}", cosmosProjects);
            _logger.LogInformation("Add Projects to CosmosDb");
            _cosmos.Projects.AddRange(cosmosProjects);
            _cosmos.SaveChanges();

            _logger.LogInformation("Move unsyncronized projects to projects in Sql");
            var sqlProject = _mapper.Map<List<Model.Sql.Project>>(sqlUnsyncProjects);
            _sql.Projects.AddRange(sqlProject);
            _sql.SaveChanges();

            // read project from Unsyncronized projects
            // insert into CosmosDb
            // move from Unsyncronized to usual
        }
    }
}
