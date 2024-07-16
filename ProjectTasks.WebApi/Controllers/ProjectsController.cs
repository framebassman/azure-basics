using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.DataAccess.Common;
using ProjectTasks.WebApi.Models;

namespace ProjectTasks.WebApi.Controllers;

[Route("[controller]")]
public class ProjectsController
{
    private ILogger<ProjectsController> _logger;
    private IProjectDataProvider _db;
    private IMapper _mapper;

    public ProjectsController(ILogger<ProjectsController> logger, IProjectDataProvider db, IMapper mapper)
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
        var projectResponse = _mapper.Map<IEnumerable<ProjectResponse>>(projects);
        return new OkObjectResult(projectResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ProjectRequest projectRequest, CancellationToken token)
    {
        _logger.LogInformation("Process {@projectRequest}", projectRequest);
        var project = await _db.CreateProjectAsync(projectRequest.Name, projectRequest.Code, token);
        var projectResponse = _mapper.Map<ProjectResponse>(project);
        return new CreatedResult("/projects", projectResponse);
    }
}
