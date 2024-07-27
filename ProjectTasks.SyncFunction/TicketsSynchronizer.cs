using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.SyncFunction
{
    public class TicketsSynchronizer
    {
        private ILogger<SyncFunction> _logger;
        private IMapper _mapper;
        private AzureSqlDbContext _sql;
        private CosmosDbContext _cosmos;

        public TicketsSynchronizer(
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

        public async Task<bool> Synchronize()
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
