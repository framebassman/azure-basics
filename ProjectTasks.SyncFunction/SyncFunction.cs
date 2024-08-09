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
        private SynchronizerAgnostic _synchronizerAgnostic;

        public SyncFunction(ILogger<SyncFunction> logger, SynchronizerAgnostic synchronizerAgnostic)
        {
            _logger = logger;
            _synchronizerAgnostic = synchronizerAgnostic;

        }

        // [Function("SyncFunction")]
        // public async Task<IActionResult> Run([TimerTrigger("*/30 * * * *")] TimerInfo timerInfo)
        [Function("SyncFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            await Task.WhenAll(
                // _synchronizerAgnostic.SynchronizeProjects(new CancellationToken()),
                _synchronizerAgnostic.SynchronizeTickets(new CancellationToken())
            );
            return new OkObjectResult("Data were synchronized");
        }
    }
}
