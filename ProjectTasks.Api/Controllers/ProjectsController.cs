using System;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult GetAll()
    {
        _logger.LogInformation("Get all projects");
        return new OkObjectResult(_db.Projects);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Project project)
    {
        try {
            _db.Projects.Add(project);
            _db.SaveChanges();
            return new OkResult();
        }
        catch (ArgumentException argumentException) {
            return new BadRequestObjectResult(argumentException.Message);
        }
    }
}
