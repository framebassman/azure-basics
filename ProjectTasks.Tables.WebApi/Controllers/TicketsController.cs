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
public class TicketsController : Controller
{
    private ILogger<TicketsController> _logger;
    private AzureSqlDbContext _db;
    private IMapper _mapper;

    public TicketsController(ILogger<TicketsController> logger, AzureSqlDbContext db, IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var tasks = await _db.Tasks.ToListAsync();
        _logger.LogInformation("Get all tasks");
        return new OkObjectResult(_mapper.Map<List<TicketResponse>>(tasks));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskAsync(int id)
    {
        _logger.LogInformation($"Get task with {id} id");
        var candidate = await _db.Tasks.FirstOrDefaultAsync(task => task.Id == id);
        if (candidate == null)
        {
            return new NotFoundObjectResult($"There is no Task with {id} id");
        }

        return new OkObjectResult(_mapper.Map<List<TicketResponse>>(candidate));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] TicketRequest ticketRequest)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ticketRequest);
        }

        var candidateProject = await _db.Projects.FirstOrDefaultAsync(p => p.Id == ticketRequest.ProjectReferenceId);
        if (candidateProject == null)
        {
            return new BadRequestObjectResult($"There is no project with {ticketRequest.ProjectReferenceId} id");
        }

        var task = new Ticket
        {
            Name = ticketRequest.Name,
            Description = ticketRequest.Description,
            Project = candidateProject
        };
        await _db.Tasks.AddAsync(task);
        await _db.SaveChangesAsync();
        return new CreatedResult("tickets", _mapper.Map<TicketResponse>(task));
    }
}
