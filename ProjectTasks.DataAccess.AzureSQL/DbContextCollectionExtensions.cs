using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.AzureSQL;

public static class DbContextCollectionExtensions
{
    public static void AddAzureSqlDataProvider(this IServiceCollection services, string connectionString)
    {
        services.AddTransient<IProjectDataProvider, AzureSqlDataProvider>();
        services.AddTransient<ITicketDataProvider, AzureSqlDataProvider>();
        services.AddDbContext<AzureSqlDbContext>(
            options => options.UseSqlServer(connectionString)
        );
    }
}
