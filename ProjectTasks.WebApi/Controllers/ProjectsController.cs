using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.Common;
using ProjectTasks.WebApi.Models;

namespace ProjectTasks.WebApi.Controllers;

[Route("[controller]")]
public class ProjectsController
{
    private ILogger<ProjectsController> _logger;
    private IProjectDataProvider<Project> _db;
    private IMapper _mapper;

    public ProjectsController(ILogger<ProjectsController> logger, IProjectDataProvider<Project> db, IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken token)
    {
        _logger.LogInformation("Get all projects");
        var projects = await _db.GetAllProjectsAsync(token);
        return new OkObjectResult(projects);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ProjectRequest projectRequest, CancellationToken token)
    {
        _logger.LogInformation("Process {@projectRequest}", projectRequest);
        var project = new Project
        {
            Name = projectRequest.Name,
            Code = projectRequest.Code,
        };
        await _db.CreateAsync(project, token);
        var projectResponse = _mapper.Map<ProjectResponse>(project);
        return new CreatedResult("/projects", projectResponse);
    }
}
