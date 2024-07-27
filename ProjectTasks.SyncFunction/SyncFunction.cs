using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProjectTasks.SyncFunction
{
    public class SyncFunction
    {
        private ILogger<SyncFunction> _logger;

        private ProjectsSynchronizer _projectsSynchronizer;
        private TicketsSynchronizer _ticketsSynchronizer;

        public SyncFunction(
            ILogger<SyncFunction> logger,
            ProjectsSynchronizer projectsSynchronizer,
            TicketsSynchronizer ticketsSynchronizer
        )
        {
            _logger = logger;
            _projectsSynchronizer = projectsSynchronizer;
            _ticketsSynchronizer = ticketsSynchronizer;
        }

        [Function("SyncFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return await SyncProjects();
        }

        public async Task<IActionResult> SyncProjects()
        {
            await Task.WhenAll(_projectsSynchronizer.Synchronize(), _ticketsSynchronizer.Synchronize());
            return new OkObjectResult("Data were synchronized");
        }
    }
}
