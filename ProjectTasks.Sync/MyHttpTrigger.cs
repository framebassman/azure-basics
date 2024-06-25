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
            // var sqlProjects = _sql.UnsyncronizedProjects.ToList();
            // _logger.LogInformation("{@sqlProjects}", sqlProjects);
            // var cosmosProjects = _cosmos.Projects.ToList();
            // _logger.LogInformation("{@cosmosProjects}", cosmosProjects);

            var sqlTasks = _sql.UnsyncronizedTasks.ToList();
            // _logger.LogInformation("{@cosmosProjects}", cosmosProjects);

            var cosmosTasks = _mapper.Map<List<Model.CosmosDb.Task>>(sqlTasks);
            _logger.LogInformation("{@cosmosTasks}", cosmosTasks);

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
