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
public class TasksController : Controller
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
        var tasks = await _db.UnsyncronizedTasks.ToListAsync();
        _logger.LogInformation("Get all tasks");
        return new OkObjectResult(_mapper.Map<List<TaskResponse>>(tasks));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskAsync(int id)
    {
        _logger.LogInformation($"Get task with {id} id");
        var candidate = await _db.UnsyncronizedTasks.FirstOrDefaultAsync(task => task.Id == id);
        if (candidate == null)
        {
            return new NotFoundObjectResult($"There is no Task with {id} id");
        }

        return new OkObjectResult(_mapper.Map<List<TaskResponse>>(candidate));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] TaskRequest taskRequest)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(taskRequest);
        }

        var candidateProject = await _db.UnsyncronizedProjects
            .FirstOrDefaultAsync(p => p.Id == taskRequest.ProjectReferenceId);
        if (candidateProject == null)
        {
            return new BadRequestObjectResult($"There is no project with {taskRequest.ProjectReferenceId} id");
        }

        var task = new UnsyncronizedTask
        {
            Name = taskRequest.Name,
            Description = taskRequest.Description,
            UnsyncronizedProject = candidateProject
        };
        await _db.UnsyncronizedTasks.AddAsync(task);
        await _db.SaveChangesAsync();
        return new CreatedResult("tasks", _mapper.Map<TaskResponse>(task));
    }
}
