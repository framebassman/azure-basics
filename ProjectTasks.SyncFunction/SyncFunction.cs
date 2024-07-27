using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;
using System.Linq;

namespace ProjectTasks.SyncFunction
{
    public class SyncFunction
    {
        private ILogger<SyncFunction> _logger;
        private IMapper _mapper;
        private AzureSqlDbContext _sql;
        private CosmosDbContext _cosmos;

        public SyncFunction(
            ILogger<SyncFunction> logger,
            IMapper mapper,
            AzureSqlDbContext sql,
            CosmosDbContext cosmos
        )
        {
            _logger = logger;
            _mapper = mapper;
            _sql = sql;
            _cosmos = cosmos;
        }

        [Function("SyncFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return await SyncProjects();
        }

        // [Function("SyncFunction")]
        // public async Task<IActionResult> Run([TimerTrigger("0 */30 * * * *")] TimerInfo timerInfo)
        // {
        //     _logger.LogInformation("Start sync");
        //     return await SyncProjects();
        // }

        public async Task<IActionResult> SyncProjects()
        {
            if (await _sql.Projects.Where(project => !project.WasSynchronized).CountAsync() == 0)
            {
                return new OkObjectResult("There is no data to sync");
            }

            List<Task> saveChangesResults = new List<Task>();
            using (var sqlTransaction = _sql.Database.BeginTransaction())
            {
                _logger.LogInformation("Get all unsync projects with tickets from SQL");
                var sqlUnsyncProjects = await _sql.Projects
                    .Where(project => !project.WasSynchronized)
                    .Include(p => p.Tickets)
                    .ToListAsync();

                var cosmosProjects = _mapper.Map<List<DataAccess.CosmosDb.Project>>(sqlUnsyncProjects);
                var cosmosTickets = new List<DataAccess.CosmosDb.Ticket>();
                _logger.LogInformation("Combine project to inject into CosmosDb and put WasSynchronizedAt into AzureSQL data");
                for (int i = 0; i < cosmosProjects.Count; i++)
                {
                    cosmosProjects[i].PartitionKey = "Test";
                    foreach (var ticket in cosmosProjects[i].Tickets)
                    {
                        ticket.PartitionKey = "Test";
                        cosmosTickets.Add(ticket);
                    }
                    sqlUnsyncProjects[i].WasSynchronized = true;
                    foreach (var unsyncTicket in sqlUnsyncProjects[i].Tickets)
                    {
                        unsyncTicket.WasSynchronized = true;
                    }
                }
                saveChangesResults.Add(_sql.SaveChangesAsync());
                _logger.LogInformation("Add projects to CosmosDb");
                await _cosmos.Projects.AddRangeAsync(cosmosProjects);
                await _cosmos.Tickets.AddRangeAsync(cosmosTickets);
                saveChangesResults.Add(_cosmos.SaveChangesAsync());

                await Task.WhenAll(saveChangesResults);
                await sqlTransaction.CommitAsync();
                return new OkObjectResult("Data was synced successfully");
            }
        }
    }
}
