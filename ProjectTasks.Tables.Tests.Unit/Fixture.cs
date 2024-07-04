using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTasks.Tables.WebApi;
using ProjectTasks.Tables.WebApi.Controllers;
using ProjectTasks.Tables.WebApi.Models;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace ProjectTasks.Api.Tests.Unit
{
    public class Fixture : TestBedFixture
    {
        protected override void AddServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddTransient<ProjectsController>()
                .AddTransient<TasksController>()
                .AddAutoMapper(typeof(Startup))
                .AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("Unit"));

        protected override ValueTask DisposeAsyncCore() => new ValueTask();

        protected override IEnumerable<TestAppSettings> GetTestAppSettings()
        {
            yield return new() { Filename = "appsettings.json", IsOptional = false };
        }
    }
}
