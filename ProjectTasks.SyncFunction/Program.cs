using System;
using System.IO;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.Common;
using ProjectTasks.DataAccess.CosmosDb;
using Serilog;

namespace ProjectTasks.SyncFunction
{
    public class Program
    {
        private static Serilog.Core.Logger _logger;

        public static void Main(string[] args)
        {
            _logger = new LoggerConfiguration()
                .ReadFrom.Configuration(BuildConfiguration())
                .CreateLogger();
            try
            {
                _logger.Information("Getting started...");
                var program = new Program();
                program.Run(args);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }

        public void Run(string[] args)
        {
            var tempBuilder = new HostBuilder()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddSerilog(_logger, true);
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
                    services.AddSingleton<IConfiguration>(BuildConfiguration());
                    services.AddSingleton<SecretsProvider>();
                }).Build();
            var secretsProvider = tempBuilder.Services.GetService<SecretsProvider>();
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddSerilog(_logger, true);
                })
                .ConfigureServices(services =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();
                    services.AddAutoMapper(typeof(Program));
                    services.AddAzureSqlDataProvider(
                        secretsProvider.Retrieve("reporting-web-api-connection-string"),
                        ServiceLifetime.Transient
                    );
                    services.AddCosmosDbDataProvider(
                        secretsProvider.Retrieve(
                            "reporting-web-api-cosmosdb-connection-string"
                        ),
                        "ProjectsTasks",
                        ServiceLifetime.Transient
                    );
                    services.AddTransient<ProjectsSynchronizer>();
                    services.AddTransient<TicketsSynchronizer>();
                })
                .Build();

            host.Run();
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("host.json", optional: false, reloadOnChange: true)
                .AddJsonFile("local.settings.json", optional: true)
                .AddJsonFile($"{CurrentEnv()}.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static string CurrentEnv()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        }
    }
}
