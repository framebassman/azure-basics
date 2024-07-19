using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.Common;
using ProjectTasks.DataAccess.CosmosDb;
using Serilog;

namespace ProjectTasks.WebApi;

public static class DbContextCollectionExtensions
{
    private static string STORAGE_TYPE = "STORAGE_TYPE";
    private static string AzureSQL = "AzureSQL";
    private static string CosmosDb = "CosmosDb";

    public static void AddDataProvider(this IServiceCollection services, string storageType, Serilog.ILogger log)
    {
        var serviceProvider = services.BuildServiceProvider();
        var secrets = serviceProvider.GetService<SecretsProvider>();
        log.Information($"{STORAGE_TYPE}: {storageType}");
        if (storageType == AzureSQL)
        {
            services.AddAzureSqlDataProvider
            (
                secrets.Retrieve("reporting-web-api-connection-string")
            );
        }
        else if (storageType == CosmosDb)
        {
            services.AddCosmosDbDataProvider
            (
                secrets.Retrieve("reporting-web-api-cosmosdb-connection-string"),
                "ProjectsTasks"
            );
        }
        else
        {
            Log.Logger.Error($"Unknown {STORAGE_TYPE} configuration");
            throw new System.ArgumentOutOfRangeException
            (
                STORAGE_TYPE,
                $"Please, define {STORAGE_TYPE} by environment variables or Asp Net Core Configuration. Application supports {AzureSQL} and {CosmosDb} storage types."
            );
        }
    }
}
