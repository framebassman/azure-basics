using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
    private IMapper _mapper;

    public ProjectsController(ILogger<ProjectsController> logger, ApplicationContext db, IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Get all projects");
        var projects = await _db.UnsyncronizedProjects.ToListAsync();
        return new OkObjectResult(_mapper.Map<List<ProjectResponse>>(projects));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ProjectRequest projectRequest)
    {
        var unsyncronizedProject = _mapper.Map<UnsyncronizedProject>(projectRequest);
        var project = _mapper.Map<Project>(projectRequest);

        await using (var sqlTransaction = await _db.Database.BeginTransactionAsync())
        {
            await _db.UnsyncronizedProjects.AddAsync(unsyncronizedProject);
            await _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();
            await sqlTransaction.CommitAsync();
        }

        var projectResponse = _mapper.Map<ProjectResponse>(unsyncronizedProject);
        return new CreatedResult("/projects", projectResponse);
    }
}
