using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.Tables.WebApi.Controllers;

[Route("[controller]")]
public class ProjectsController
{
    private ILogger<ProjectsController> _logger;
    private AzureSqlDbContext _db;
    private IMapper _mapper;

    public ProjectsController(ILogger<ProjectsController> logger, AzureSqlDbContext db, IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Get all projects");
        var projects = await _db.Projects.ToListAsync();
        return new OkObjectResult(_mapper.Map<List<ProjectResponse>>(projects));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ProjectRequest projectRequest)
    {
        var project = new Project
        {
            Name = projectRequest.Name,
            Code = projectRequest.Code,
        };
        await _db.Projects.AddAsync(project);
        await _db.SaveChangesAsync();
        var projectResponse = _mapper.Map<ProjectResponse>(project);
        return new CreatedResult("/projects", projectResponse);
    }
}
