using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ProjectTasks.Sync.Model.Sql;
using ProjectTasks.Sync.Model.CosmosDb;

namespace ProjectTasks.Sync
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
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureServices(services => {
                    services.AddAutoMapper(typeof(Program));
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();
                    services.AddDbContext<SqlContext>(
                        options => options.UseSqlServer(BuildConfiguration().GetConnectionString("Sql"))
                    );
                    services.AddDbContext<CosmosDbContext>(
                        options => options.UseCosmos(
                            BuildConfiguration().GetConnectionString("CosmosDb"),
                            "ProjectsTasks"
                        )
                    );
                })
                // .ConfigureLogging((hostingContext, logging) =>
                // {
                //     logging.AddSerilog(_logger, true);
                // })
                .Build();

            host.Run();
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("host.json", optional: false, reloadOnChange: true)
                // .AddJsonFile($"appsettings.{CurrentEnv()}.json", optional: true)
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
