using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.CosmosDb;

public static class DbContextCollectionExtensions
{
    public static void AddCosmosDbDataProvider(this IServiceCollection services, string connectionString, string databaseName, ServiceLifetime lifetime)
    {
        services.AddTransient<IProjectDataProvider, CosmosDbDataProvider>();
        services.AddTransient<ITicketDataProvider, CosmosDbDataProvider>();
        services.AddTransient<CosmosDbDataProvider>();
        services.AddDbContext<CosmosDbContext>(
            options => options.UseCosmos(connectionString, databaseName),
            lifetime
        );
    }
}
