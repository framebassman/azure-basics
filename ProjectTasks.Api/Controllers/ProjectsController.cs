using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

    [HttpGet("Test")]
    public async Task<IActionResult> Move()
    {
        var unsyncProjects = _db.UnsyncronizedProjects
            .Include(p => p.UnsyncronizedTasks)
            .ToList();

        _db.Projects.AddRange(GetProjects(unsyncProjects));
        _db.SaveChanges();
        return new OkResult();
    }

    public List<Project> GetProjects(List<UnsyncronizedProject> unsyncProjects)
    {
        List<Project> result = new List<Project>();
        foreach (var project in unsyncProjects)
        {
            List<Models.Task> tasks = new List<Models.Task>();
            foreach (var task in project.UnsyncronizedTasks)
            {
                tasks.Add(new Models.Task { Name = task.Name, Description = task.Description });
            }
            result.Add(new Project { Name = project.Name, Code = project.Code, Tasks = tasks });
        }
        return result;
    }
}
