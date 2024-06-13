using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectTasks.CosmosDb.Models;

namespace ProjectTasks.CosmosDb.Controllers
{
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
            var projects = await _db.Projects.ToListAsync();
            return new OkObjectResult(_mapper.Map<List<ProjectResponse>>(projects));
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create project with task");
            var project = new Project
            {
                Id = 2,
                PartitionKey = "TestProject2",
                Name = "TestProject2",
                Code = "TPJ",
                Tasks = new List<Models.Task>()
            };
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            var projectInDb = _db.Projects.First(p => p.Id == 2);
            var task = new Models.Task
            {
                Id = 2,
                PartitionKey = "TestTask2",
                Name = "TestTask2",
                Description = "TestDescription2",
                ProjectId = 2,
                Project = projectInDb
            };
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            return new OkResult();
        }
    }
}
