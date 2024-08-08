using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProjectTasks.SyncFunction.First;

namespace ProjectTasks.SyncFunction
{
    public class SyncFunction
    {
        private ILogger<SyncFunction> _logger;
        private ProjectsSynchronizer _projectsSynchronizer;
        private TicketsSynchronizer _ticketsSynchronizer;
        private SynchronizerAgnostic _synchronizerAgnostic;

        public SyncFunction(
            ILogger<SyncFunction> logger,
            ProjectsSynchronizer projectsSynchronizer,
            TicketsSynchronizer ticketsSynchronizer,
            SynchronizerAgnostic synchronizerAgnostic
        )
        {
            _logger = logger;
            _projectsSynchronizer = projectsSynchronizer;
            _ticketsSynchronizer = ticketsSynchronizer;
            _synchronizerAgnostic = synchronizerAgnostic;

        }

        // [Function("SyncFunction")]
        // public async Task<IActionResult> Run([TimerTrigger("*/30 * * * *")] TimerInfo timerInfo)
        [Function("SyncFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // await Task.WhenAll(_projectsSynchronizer.SynchronizeAsync(), _ticketsSynchronizer.SynchronizeAsync());
            var result = await _synchronizerAgnostic.SynchronizeProjects(new CancellationToken());
            var message = result ? "Data were synchronized" : "There is no data to synchronize";
            return new OkObjectResult(message);
        }
    }
}
