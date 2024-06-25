using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        var project = new UnsyncronizedProject
        {
            Name = projectRequest.Name,
            Code = projectRequest.Code,
        };
        await _db.UnsyncronizedProjects.AddAsync(project);
        await _db.SaveChangesAsync();
        var projectResponse = _mapper.Map<ProjectResponse>(project);
        return new CreatedResult("/projects", projectResponse);
    }
}
