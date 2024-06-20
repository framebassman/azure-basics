using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetAll([FromQuery] bool? withTasks)
        {
            if (withTasks.HasValue && withTasks.Value)
            {
                _logger.LogInformation("Get all projects with tasks");
                var projects = await _db.Projects.ToListAsync();
                projects.ForEach(project => {
                    _db.Entry(project)
                        .Collection(b => b.Tasks)
                        .Load();
                });
                return new OkObjectResult(_mapper.Map<List<ProjectResponse>>(projects));
            }
            else
            {
                _logger.LogInformation("Get all projects without tasks");
                var projects = await _db.Projects.ToListAsync();
                return new OkObjectResult(_mapper.Map<List<ProjectResponse>>(projects));
            }
        }
    }
}
