using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectTasks.DataAccess.Common;
using ProjectTasks.Presentation.Common;
using Serilog;

namespace ProjectTasks.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment env)
        {
            Environment = env;
            Configuration = Program.BuildConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(opt => opt.LowercaseUrls = true);
            services.AddHealthChecks();
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.AddSwaggerGen();
            services.AddSerilog();
            services.AddHttpLogging(o => { });
            services.AddSingleton<SecretsProvider>();
            services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
            services.AddSingleton(Configuration);
            services.AddDataProvider(Configuration["STORAGE_TYPE"], Log.Logger, ServiceLifetime.Scoped);
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
            app.UseHttpLogging();
        }
    }
}
