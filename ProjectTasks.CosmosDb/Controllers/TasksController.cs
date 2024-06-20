using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetAll([FromQuery( Name = "projectId")] string projectIdentifier)
        {
            List<Models.Task> tasks;
            if (string.IsNullOrEmpty(projectIdentifier))
            {
                _logger.LogInformation("Get all tasks");
                tasks = await _db.Tasks.ToListAsync();
                return new OkObjectResult(_mapper.Map<List<TaskResponse>>(tasks));
            }

            _logger.LogInformation("Get all tasks for projectId: {@projectId}", projectIdentifier);
            int projectId;
            bool success = int.TryParse(projectIdentifier, out projectId);
            if (!success)
            {
                return new BadRequestObjectResult($"There is no Project with {projectIdentifier} id");
            }

            tasks = await _db.Tasks
                .Where(task => task.ProjectId == projectId)
                .ToListAsync();
            return new OkObjectResult(_mapper.Map<List<TaskResponse>>(tasks));
        }
    }
}
