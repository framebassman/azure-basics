using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.CosmosDb;
using Serilog;

namespace ProjectTasks.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        private SecretClientOptions _secretClientOptions;
        private TokenCredential _azureCredentials;

        public Startup(IWebHostEnvironment env)
        {
            Environment = env;
            Configuration = Program.BuildConfiguration();
            _secretClientOptions = new SecretClientOptions
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                }
            };
            _azureCredentials = new DefaultAzureCredential();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(opt => opt.LowercaseUrls = true);
            services.AddHealthChecks();
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.AddSwaggerGen();
            services.AddSerilog();
            if (Configuration["STORAGE_TYPE"] == "AzureSQL")
            {
                Log.Logger.Information("StorageType: AzureSQL");
                services.AddAzureSqlDataProvider(RetrieveSecret("reporting-web-api-connection-string"));
            }
            else if (Configuration["STORAGE_TYPE"] == "CosmosDb")
            {
                Log.Logger.Information("StorageType: CosmosDb");
                services.AddDbContext<CosmosDbContext>(
                    options => options.UseCosmos(
                        RetrieveSecret("reporting-web-api-cosmosdb-connection-string"),
                        "ProjectsTasks"
                    )
                );
            }
            else
            {
                Log.Logger.Information("StorageType: InMemoryDatabase");
                services.AddDbContext<AzureSqlDbContext>(
                    options => options.UseInMemoryDatabase("Data")
                );
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");
            });
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
        }

        private string RetrieveSecret(string secretKey)
        {
            var keyVaultUrl = Configuration["AppKeyVault:Endpoint"];
            var keyVaultClient = new SecretClient(new Uri(keyVaultUrl), _azureCredentials, _secretClientOptions);
            KeyVaultSecret secret = keyVaultClient.GetSecret(secretKey);
            return secret.Value;
        }
    }
}
