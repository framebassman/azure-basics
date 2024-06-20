using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectTasks.CosmosDb.Models;

namespace ProjectTasks.CosmosDb.Controllers
{
    [Route("[controller]")]
    public class TasksController
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
            _logger.LogInformation("Get all tasks");
            var tasks = await _db.Tasks.ToListAsync();
            return new OkObjectResult(_mapper.Map<List<TaskResponse>>(tasks));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync()
        {
            var project = new Models.Project {
                Id = 1,
                PartitionKey = "Test",
                Code = "TST",
                Name = "Test"
            };
            await _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();
            project = _db.Projects.First();

            var task = new Models.Task {
                Id = 1,
                PartitionKey = "1",
                Name = "Second",
                Description = "Second",
                Project = project
            };
            await _db.Tasks.AddAsync(task);
            await _db.SaveChangesAsync();
            return new CreatedResult("tasks", task);
        }
    }
}
