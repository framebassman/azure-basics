using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ProjectTasks.Sync
{
    public class ProjectTasksSync
    {
        private readonly ILogger _logger;

        public ProjectTasksSync(ILogger<ProjectTasksSync> logger)
        {
            _logger = logger;
        }

        [Function("ProjectTasksSync")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");

            }
        }
    }
}
