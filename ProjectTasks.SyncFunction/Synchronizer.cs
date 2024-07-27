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
    public abstract class Synchronizer<S, T>
        where S : class, ISynchronizable
        where T : class, IWithPartitionKey
    {
        private ILogger<Synchronizer<S, T>> _logger;
        private IMapper _mapper;
        protected AzureSqlDbContext _sql;
        protected CosmosDbContext _cosmos;

        public Synchronizer(
            ILogger<Synchronizer<S, T>> logger,
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

        public abstract Task<bool> SynchronizeAsync();

        protected async Task<bool> SynchronizeEntities(string entitiesName, DbSet<S> sqlEntities, DbSet<T> cosmosEntities)
        {
            if (await sqlEntities.Where(e => !e.WasSynchronized).CountAsync() == 0)
            {
                _logger.LogInformation($"There is no {entitiesName} to sync");
                return true;
            }

            using (var sqlTransaction = _sql.Database.BeginTransaction())
            {
                var saveChangesResults = new List<Task>();
                _logger.LogInformation($"Get all unsync {entitiesName} with tickets from SQL");
                var sqlUnsync = await sqlEntities
                    .Where(e => !e.WasSynchronized)
                    .ToListAsync();

                var cosmosSync = _mapper.Map<List<T>>(sqlUnsync);
                cosmosSync.ForEach(e => e.PartitionKey = "Test");
                await cosmosEntities.AddRangeAsync(cosmosSync);
                saveChangesResults.Add(_cosmos.SaveChangesAsync());

                sqlUnsync.ForEach(project => project.WasSynchronized = true);
                saveChangesResults.Add(_sql.SaveChangesAsync());

                await Task.WhenAll(saveChangesResults);
                await sqlTransaction.CommitAsync();
                return true;
            }
        }
    }
}
