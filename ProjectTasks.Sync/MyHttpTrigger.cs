using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.Sync.Model.Sql;
using ProjectTasks.Sync.Model.CosmosDb;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            SetupEnvironment();
            await SyncProjects();
            return new OkObjectResult("Welcome to Azure Functions!");
        }

        public async Task<IActionResult> SyncProjects()
        {
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
                var sqlProject = GetProjectsWithTasks(sqlUnsyncProjects);
                await _sql.Projects.AddRangeAsync(sqlProject);
                _sql.UnsyncronizedProjects.RemoveRange(sqlUnsyncProjects);
                saveChangesResults.Add(_sql.SaveChangesAsync());

                var finishedTask = System.Threading.Tasks.Task.WhenAll(saveChangesResults);
                await finishedTask.ContinueWith(x => { });
                sqlTransaction.Commit();
                return new OkResult();
            }
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
