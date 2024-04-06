using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectTasks.Api.Models;

namespace ProjectTasks.Api.Controllers;

[Route("[controller]")]
public class ProjectsController
{
    private ILogger<ProjectsController> _logger;
    private ApplicationContext _db;

    public ProjectsController(ILogger<ProjectsController> logger, ApplicationContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Get all projects");
        return new OkObjectResult(await _db.Projects.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Project project)
    {
        try {
            await _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();
            return new OkResult();
        }
        catch (ArgumentException argumentException) {
            return new BadRequestObjectResult(argumentException.Message);
        }
    }
}
