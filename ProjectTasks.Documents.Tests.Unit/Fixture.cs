using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTasks.DataAccess.CosmosDb;
using ProjectTasks.Documents.WebApi;
using ProjectTasks.Documents.WebApi.Controllers;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace ProjectTasks.Documents.Tests.Unit;

public class Fixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
        => services
            .AddTransient<ProjectsController>()
            .AddTransient<TicketsController>()
            .AddAutoMapper(typeof(Startup))
            .AddDbContext<CosmosDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = false };
    }

    protected override ValueTask DisposeAsyncCore() => new ValueTask();
}
