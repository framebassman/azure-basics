using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.Sync.Model.Sql;
using ProjectTasks.Sync.Model.CosmosDb;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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

        public void SyncProjects()
        {
            _logger.LogInformation("");
            _logger.LogInformation("Get All unsync projects from SQL");
            var sqlUnsyncProjects = _sql.UnsyncronizedProjects
                .Include(p => p.Tasks)
                .ToList();

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
            _cosmos.Projects.AddRange(cosmosProjects);
            _cosmos.Tasks.AddRange(cosmosTasks);
            _cosmos.SaveChanges();

            _logger.LogInformation("");
            _logger.LogInformation("Move unsyncronized projects to projects in Sql");
            var sqlProject = GetProjectsWithTasks(sqlUnsyncProjects);
            _sql.Projects.AddRange(sqlProject);
            _sql.UnsyncronizedProjects.RemoveRange(sqlUnsyncProjects);
            _sql.SaveChanges();

            // var sqlUnsyncTasks = sqlUnsyncProjects.SelectMany(p => p.Tasks);
            // var sqlTasks = _mapper.Map<List<Model.Sql.Task>>(sqlUnsyncTasks);
            // _sql.Tasks.AddRange(sqlTasks);

            // _logger.LogInformation("Remove unsync entities");
            // _sql.UnsyncronizedProjects.RemoveRange(sqlUnsyncProjects);
            // // _sql.UnsyncronizedTasks.RemoveRange(sqlUnsyncTasks);
            // _sql.SaveChanges();
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
                new UnsyncronizedProject { Name = "Test", Code = "TST" },
                new UnsyncronizedProject { Name = "Some", Code = "SM" }
            };
            _sql.UnsyncronizedProjects.AddRange(projects);
            _sql.SaveChanges();

            projects = _sql.UnsyncronizedProjects.ToList();
            var tasks = new List<UnsyncronizedTask>()
            {
                new UnsyncronizedTask { Name = "SomeTask", Description = "SomeDesc", ProjectReferenceId = projects[0].Id },
                new UnsyncronizedTask { Name = "TestTask", Description = "TestDesc", ProjectReferenceId = projects[1].Id }
            };
            _sql.UnsyncronizedTasks.AddRange(tasks);
            _sql.SaveChanges();
        }

        public List<Model.Sql.Project> GetProjectsWithTasks(List<UnsyncronizedProject> unsyncProjects)
        {
            List<Model.Sql.Project> result = new List<Model.Sql.Project>();
            foreach (var project in unsyncProjects)
            {
                List<Model.Sql.Task> tasks = new List<Model.Sql.Task>();
                foreach (var task in project.Tasks)
                {
                    tasks.Add(new Model.Sql.Task { Name = task.Name, Description = task.Description });
                }
                result.Add(new Model.Sql.Project { Name = project.Name, Code = project.Code, Tasks = tasks });
            }
            return result;
        }
    }
}
