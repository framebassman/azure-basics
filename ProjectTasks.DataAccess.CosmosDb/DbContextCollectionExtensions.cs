using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.CosmosDb;

public static class DbContextCollectionExtensions
{
    public static void AddCosmosDbDataProvider(this IServiceCollection services, string connectionString, string databaseName)
    {
        services.AddTransient<IProjectDataProvider, CosmosDbDataProvider>();
        services.AddTransient<ITicketDataProvider, CosmosDbDataProvider>();
        services.AddDbContext<CosmosDbContext>(
            options => options.UseCosmos(connectionString, databaseName)
        );
    }
}
