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
            SetupEnvironment();
            SyncProjects();
            return new OkObjectResult("Welcome to Azure Functions!");
        }

        public void SyncTasks()
        {

        }

        public void SyncProjects()
        {
            _logger.LogInformation("");
            _logger.LogInformation("Get All unsync projects from SQL");
            var sqlUnsyncProjects = _sql.UnsyncronizedProjects.ToList();

            var cosmosProjects = _mapper.Map<List<Model.CosmosDb.Project>>(sqlUnsyncProjects);
            foreach (var project in cosmosProjects)
            {
                project.PartitionKey = "Test";
            }
            _logger.LogInformation("");
            _logger.LogInformation("Add Projects to CosmosDb");
            _cosmos.Projects.AddRange(cosmosProjects);
            _cosmos.SaveChanges();

            _logger.LogInformation("");
            _logger.LogInformation("Move unsyncronized projects to projects in Sql");
            var sqlProject = _mapper.Map<List<Model.Sql.Project>>(sqlUnsyncProjects);
            _sql.Projects.AddRange(sqlProject);
            _sql.UnsyncronizedProjects.RemoveRange(sqlUnsyncProjects);
            _sql.SaveChanges();
        }

        public void SetupEnvironment()
        {
            _cosmos.Projects.RemoveRange(_cosmos.Projects.ToList());
            _cosmos.Tasks.RemoveRange(_cosmos.Tasks.ToList());
            _cosmos.SaveChanges();

            _sql.Projects.RemoveRange(_sql.Projects.ToList());
            _sql.Tasks.RemoveRange(_sql.Tasks.ToList());
            _sql.UnsyncronizedProjects.RemoveRange(_sql.UnsyncronizedProjects.ToList());
            _sql.UnsyncronizedTasks.RemoveRange(_sql.UnsyncronizedTasks.ToList());

            var projects = new List<UnsyncronizedProject>()
            {
                new UnsyncronizedProject
                {
                    Name = "Test",
                    Code = "TST",
                    Tasks = new List<UnsyncronizedTask>()
                    {
                        new UnsyncronizedTask { Name = "TestTask", Description = "TestDesc" }
                    }
                },
                new UnsyncronizedProject
                {
                    Name = "Some",
                    Code = "SM",
                    Tasks = new List<UnsyncronizedTask>()
                    {
                        new UnsyncronizedTask { Name = "SomeTask", Description = "SomeDesc" }
                    }
                }
            };
            _sql.UnsyncronizedProjects.AddRange(projects);
            _sql.SaveChanges();
        }
    }
}
