using System;
using System.IO;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.SyncFunction;
using ProjectTasks.Presentation.Common;
using Serilog;
using ProjectTasks.DataAccess.Common;
using ProjectTasks.SyncFunction.First;

var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("host.json", optional: false, reloadOnChange: true)
    .AddJsonFile("local.settings.json", optional: true)
    .AddJsonFile($"{currentEnv}.settings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();


Serilog.Core.Logger logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var host = new HostBuilder()
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddSerilog(logger, true);
    })
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddAutoMapper(typeof(Program));
        services.AddTransient<SecretsProvider>();
        services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
        services.AddSingleton(configuration);
        services.AddDataProvider("AzureSQL", Log.Logger, ServiceLifetime.Transient);
        services.AddDataProvider("CosmosDb", Log.Logger, ServiceLifetime.Transient);
        services.AddTransient<ProjectsSynchronizer>();
        services.AddTransient<TicketsSynchronizer>();
    })
    .Build();

try
{
    logger.Information("Getting started...");
    host.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
