using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public async Task<IActionResult> GetAllAsync([FromQuery] int? id)
    {
        if (!id.HasValue)
        {
            var tasks = await _db.Tasks.ToListAsync();
            _logger.LogInformation("Get all tasks");
            return new OkObjectResult(_mapper.Map<List<TaskResponse>>(tasks));
        }
        else
        {
            return await GetTaskAsync(id.GetValueOrDefault());
        }

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskAsync(int id)
    {
        _logger.LogInformation($"Get task with {id} id");
        var tasks = await _db.Tasks.ToListAsync();
        try
        {
            return new OkObjectResult(
                _mapper.Map<TaskResponse>(
                    tasks.First(task => task.Id == id)
                )
            );
        }
        catch
        {
            return new NotFoundObjectResult($"There is no Task with {id} id");
        }
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
