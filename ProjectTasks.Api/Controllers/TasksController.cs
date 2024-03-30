using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectTasks.Api.Models;

namespace ProjectTasks.Api.Controllers;

[Route("[controller]")]
public class TasksController
{
    private ILogger<TasksController> _logger;
    private ApplicationContext _db;

    public TasksController(ILogger<TasksController> logger, ApplicationContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("Get all tasks");
        return new OkObjectResult(_db.Tasks);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Task task)
    {
        var candidateProject = _db.Projects.FirstOrDefault(p => p.Id == task.ProjectReferenceId);
        if (candidateProject == null)
        {
            return new BadRequestObjectResult($"There is no project with {task.ProjectReferenceId} id");
        }

        try {
            task.Project = candidateProject;
            _db.Tasks.Add(task);
            _db.SaveChanges();
            return new OkResult();
        }
        catch (ArgumentException argumentException) {
            return new BadRequestObjectResult(argumentException.Message);
        }
    }
}
