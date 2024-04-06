using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    private IMapper _mapper;

    public TasksController(ILogger<TasksController> logger, ApplicationContext db, IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Get all tasks");
        var tasks = await _db.Tasks.ToListAsync();
        return new OkObjectResult(_mapper.Map<List<TaskResponse>>(tasks));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] TaskRequest taskRequest)
    {
        var candidateProject = await _db.Projects.FirstOrDefaultAsync(p => p.Id == taskRequest.ProjectReferenceId);
        if (candidateProject == null)
        {
            return new BadRequestObjectResult($"There is no project with {taskRequest.ProjectReferenceId} id");
        }

        try
        {
            var task = new Task
            {
                Name = taskRequest.Name,
                Description = taskRequest.Description,
                Project = candidateProject
            };
            await _db.Tasks.AddAsync(task);
            await _db.SaveChangesAsync();
            return new CreatedResult("tasks", _mapper.Map<TaskResponse>(task));
        }
        catch (ArgumentException argumentException) {
            return new BadRequestObjectResult(argumentException.Message);
        }
    }
}
