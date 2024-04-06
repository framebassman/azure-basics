using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectTasks.Api.Models;
using Task = ProjectTasks.Api.Models.Task;

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
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Get all tasks");
        return new OkObjectResult(await _db.Tasks.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Task task)
    {
        var candidateProject = await _db.Projects.FirstOrDefaultAsync(p => p.Id == task.ProjectReferenceId);
        if (candidateProject == null)
        {
            return new BadRequestObjectResult($"There is no project with {task.ProjectReferenceId} id");
        }

        try {
            task.Project = candidateProject;
            await _db.Tasks.AddAsync(task);
            await _db.SaveChangesAsync();
            return new OkResult();
        }
        catch (ArgumentException argumentException) {
            return new BadRequestObjectResult(argumentException.Message);
        }
    }
}
