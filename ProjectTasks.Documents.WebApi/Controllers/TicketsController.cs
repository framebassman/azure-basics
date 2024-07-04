using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTasks.DataAccess.CosmosDb;
using ProjectTasks.Documents.WebApi.Models;

namespace ProjectTasks.Documents.WebApi.Controllers
{
    [Route("[controller]")]
    public class TicketsController
    {
        private ILogger<TicketsController> _logger;
        private CosmosDbContext _db;
        private IMapper _mapper;

        public TicketsController(ILogger<TicketsController> logger, CosmosDbContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery( Name = "projectId")] string projectIdentifier)
        {
            if (string.IsNullOrEmpty(projectIdentifier))
            {
                _logger.LogInformation("Get all tasks");
                var allTickets = await _db.Tickets.ToListAsync();
                return new OkObjectResult(_mapper.Map<List<TicketResponse>>(allTickets));
            }

            _logger.LogInformation("Get all tasks for projectId: {@projectId}", projectIdentifier);
            int projectId;
            bool success = int.TryParse(projectIdentifier, out projectId);
            if (!success)
            {
                return new BadRequestObjectResult($"There is no Project with {projectIdentifier} id");
            }

            var project = _db.Projects
                .Include(p => p.Tickets)
                .FirstOrDefault(p => p.Id == projectId);
            if (project == null)
            {
                return new BadRequestObjectResult($"There is no Project with {projectIdentifier} id");
            }

            var tickets = project.Tickets.ToList();
            return new OkObjectResult(_mapper.Map<List<TicketResponse>>(tickets));
        }
    }
}
