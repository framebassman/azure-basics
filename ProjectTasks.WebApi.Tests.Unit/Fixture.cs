using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.Common;
using ProjectTasks.WebApi.Controllers;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace ProjectTasks.WebApi.Tests.Unit
{
    public class Fixture : TestBedFixture
    {
        protected override void AddServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddTransient<ProjectsController>()
                .AddTransient<TicketsController>()
                .AddAutoMapper(typeof(Startup))
                .AddTransient<IProjectDataProvider<Project>, AzureSqlDataProvider>()
                .AddTransient<ITicketDataProvider<Ticket>, AzureSqlDataProvider>()
                .AddDbContext<AzureSqlDbContext>(
                    options => options.UseInMemoryDatabase(Guid.NewGuid().ToString())
                );

        protected override ValueTask DisposeAsyncCore() => new ValueTask();

        protected override IEnumerable<TestAppSettings> GetTestAppSettings()
        {
            yield return new() { Filename = "appsettings.json", IsOptional = false };
        }
    }
}
