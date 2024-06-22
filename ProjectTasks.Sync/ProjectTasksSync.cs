using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProjectsTasksSync
{
    public class ProjectsTasksSync
    {
        [FunctionName("PopulateCosmosDbFunction")]
        public async System.Threading.Tasks.Task Run(
            [TimerTrigger("*/30 * * * * *")]TimerInfo myTimer,
            ILogger log
        )
        {
            log.LogInformation("Hello, world");
        }
    }
}
