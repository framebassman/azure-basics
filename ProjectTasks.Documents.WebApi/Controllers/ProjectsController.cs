using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTasks.DataAccess.CosmosDb;
using ProjectTasks.Documents.WebApi.Models;

namespace ProjectTasks.Documents.WebApi.Controllers
{
    [Route("[controller]")]
    public class ProjectsController
    {
        private ILogger<ProjectsController> _logger;
        private CosmosDbContext _db;
        private IMapper _mapper;

        public ProjectsController(ILogger<ProjectsController> logger, CosmosDbContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? withTickets)
        {
            if (withTickets.HasValue && withTickets.Value)
            {
                _logger.LogInformation("Get all projects with tasks");
                var projectsWithTickets = await _db.Projects.ToListAsync();
                projectsWithTickets.ForEach(project => {
                    _db.Entry(project)
                        .Collection(b => b.Tickets)
                        .Load();
                });
                return new OkObjectResult(_mapper.Map<List<ProjectResponse>>(projectsWithTickets));
            }

            _logger.LogInformation("Get all projects without tasks");
            var projects = await _db.Projects.ToListAsync();
            return new OkObjectResult(_mapper.Map<List<ProjectResponse>>(projects));
        }
    }
}
