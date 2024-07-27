using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

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

        public async Task<IActionResult> SyncProjects()
        {
            await SynchronizeProjects();
            await SynchronizeTickets();
            return new OkObjectResult("Data were synchronized");
        }

        private async Task<bool> SynchronizeProjects()
        {
            if (await _sql.Projects.Where(project => !project.WasSynchronized).CountAsync() == 0)
            {
                _logger.LogInformation("There is no projects to sync");
                return true;
            }

            using (var sqlTransaction = _sql.Database.BeginTransaction())
            {
                var saveChangesResults = new List<Task>();
                _logger.LogInformation("Get all unsync projects with tickets from SQL");
                var sqlUnsyncProjects = await _sql.Projects
                    .Where(project => !project.WasSynchronized)
                    .ToListAsync();

                var cosmosProjects = _mapper.Map<List<DataAccess.CosmosDb.Project>>(sqlUnsyncProjects);
                cosmosProjects.ForEach(project => project.PartitionKey = "Test");
                await _cosmos.Projects.AddRangeAsync(cosmosProjects);
                saveChangesResults.Add(_cosmos.SaveChangesAsync());

                sqlUnsyncProjects.ForEach(project => project.WasSynchronized = true);
                saveChangesResults.Add(_sql.SaveChangesAsync());

                await Task.WhenAll(saveChangesResults);
                await sqlTransaction.CommitAsync();
                return true;
            }
        }

        private async Task<bool> SynchronizeTickets()
        {
            if (await _sql.Tickets.Where(ticket => !ticket.WasSynchronized).CountAsync() == 0)
            {
                _logger.LogInformation("There is no tickets to sync");
                return true;
            }

            using (var sqlTransaction = _sql.Database.BeginTransaction())
            {
                var saveChangesResults = new List<Task>();
                _logger.LogInformation("Get all unsync tickets with tickets from SQL");
                var sqlUnsyncTickets = await _sql.Tickets
                    .Where(ticket => !ticket.WasSynchronized)
                    .ToListAsync();

                var cosmosTickets = _mapper.Map<List<DataAccess.CosmosDb.Ticket>>(sqlUnsyncTickets);
                cosmosTickets.ForEach(ticket => ticket.PartitionKey = "Test");
                await _cosmos.Tickets.AddRangeAsync(cosmosTickets);
                saveChangesResults.Add(_cosmos.SaveChangesAsync());

                sqlUnsyncTickets.ForEach(ticket => ticket.WasSynchronized = true);
                saveChangesResults.Add(_sql.SaveChangesAsync());

                await Task.WhenAll(saveChangesResults);
                await sqlTransaction.CommitAsync();
                return true;
            }
        }
    }
}
