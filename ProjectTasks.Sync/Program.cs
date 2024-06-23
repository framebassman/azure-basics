using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace ProjectTasks.Sync
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(BuildConfiguration())
                .CreateLogger();
            try
            {
                Serilog.Log.Logger.Information("Getting started...");
                var host = new HostBuilder()
                    .ConfigureFunctionsWebApplication()
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(BuildConfiguration())
                            .CreateLogger();

                        logging.AddSerilog(Log.Logger, true);
                    })
                    .ConfigureServices(services =>
                    {
                        services.ConfigureFunctionsApplicationInsights();
                        services.AddHealthChecks();
                    })
                    .Build();

                host.Run();
            }
            catch (Exception ex)
            {
                Serilog.Log.Logger.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{CurrentEnv()}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static string CurrentEnv()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        }
    }
}
