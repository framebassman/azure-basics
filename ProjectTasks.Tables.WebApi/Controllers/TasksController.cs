using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using ProjectTasks.Tables.WebApi.Models;
using Models_Task = ProjectTasks.Tables.WebApi.Models.Task;
using Task = ProjectTasks.Tables.WebApi.Models.Task;

namespace ProjectTasks.Tables.WebApi.Controllers;

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
        var tasks = await _db.Tasks.ToListAsync();
        _logger.LogInformation("Get all tasks");
        return new OkObjectResult(_mapper.Map<List<TaskResponse>>(tasks));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskAsync(int id)
    {
        _logger.LogInformation($"Get task with {id} id");
        var candidate = await _db.Tasks.FirstOrDefaultAsync(task => task.Id == id);
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

        var candidateProject = await _db.Projects.FirstOrDefaultAsync(p => p.Id == taskRequest.ProjectReferenceId);
        if (candidateProject == null)
        {
            return new BadRequestObjectResult($"There is no project with {taskRequest.ProjectReferenceId} id");
        }

        var task = new Models_Task
        {
            Name = taskRequest.Name,
            Description = taskRequest.Description,
            Project = candidateProject
        };
        await _db.Tasks.AddAsync(task);
        await _db.SaveChangesAsync();
        return new CreatedResult("tasks", _mapper.Map<TaskResponse>(task));
    }
}
