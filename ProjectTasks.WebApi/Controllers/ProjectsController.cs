using Microsoft.AspNetCore.Mvc;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.WebApi.Controllers;

[Route("[controller]")]
public class ProjectsController
{
    private ILogger<ProjectsController> _logger;
    private IProjectDataProvider<Project> _db;

    public ProjectsController(ILogger<ProjectsController> logger, IProjectDataProvider<Project> db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken token)
    {
        var projects = await _db.GetAllProjectsAsync(token);
        return new OkObjectResult(projects);
    }
}
