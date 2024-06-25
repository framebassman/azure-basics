using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.Sync.Model.Sql;

namespace ProjectTasks.Sync
{
    public class MyHttpTrigger
    {
        private readonly ILogger<MyHttpTrigger> _logger;
        private SqlContext _sql;

        public MyHttpTrigger(ILogger<MyHttpTrigger> logger, SqlContext sql)
        {
            _logger = logger;
            _sql = sql;
        }

        [Function("MyHttpTrigger")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var projects = _sql.UnsyncronizedProjects.ToList();
            _logger.LogInformation("{@projects}", projects);
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
