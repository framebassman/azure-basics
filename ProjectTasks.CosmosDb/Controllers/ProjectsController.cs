using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            var allTasks = _db.Tasks.ToList();

            _logger.LogInformation("Get all projects");
            var projects = await _db.Projects.ToListAsync();
            projects.ForEach(project => {
                 _db.Entry(project)
                    .Collection(b => b.Tasks)
                    .Load();
            });
            return new OkObjectResult(projects);
        }
    }
}
