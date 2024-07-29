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
            ILogger<SyncFunction> logger
            // ProjectsSynchronizer projectsSynchronizer,
            // TicketsSynchronizer ticketsSynchronizer
        )
        {
            _logger = logger;
            // _projectsSynchronizer = projectsSynchronizer;
            // _ticketsSynchronizer = ticketsSynchronizer;
        }

        // [Function("SyncFunction")]
        // public async Task<IActionResult> Run([TimerTrigger("0 */2 * * * *")] TimerInfo timerInfo)
        [Function("SyncFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // _projectsSynchronizer.SynchronizeAsync();
            // _ticketsSynchronizer.SynchronizeAsync();
            return new OkObjectResult("Data were synchronized");
        }

        public async Task<IActionResult> SyncProjects()
        {
            await Task.WhenAll(_projectsSynchronizer.SynchronizeAsync(), _ticketsSynchronizer.SynchronizeAsync());
            return new OkObjectResult("Data were synchronized");
        }
    }
}
