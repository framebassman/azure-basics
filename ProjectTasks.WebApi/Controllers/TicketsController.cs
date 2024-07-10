using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.DataAccess.Common;
using ProjectTasks.WebApi.Models;

namespace ProjectTasks.WebApi.Controllers;

[Route("[controller]")]
public class TicketsController : Controller
{
    private ILogger<TicketsController> _logger;
    private IProjectDataProvider _projectsDb;
    private ITicketDataProvider<Ticket> _ticketsDb;
    private IMapper _mapper;

    public TicketsController(ILogger<TicketsController> logger, IProjectDataProvider projectsDb, ITicketDataProvider<Ticket> ticketsDb, IMapper mapper)
    {
        _logger = logger;
        _ticketsDb = ticketsDb;
        _projectsDb = projectsDb;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken token)
    {
        _logger.LogInformation("Get all tasks");
        var tasks = await _ticketsDb.GetAllTicketsAsync(token);
        return new OkObjectResult(_mapper.Map<List<TicketResponse>>(tasks));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskAsync(int id, CancellationToken token)
    {
        _logger.LogInformation($"Get task with {id} id");
        var candidate = await _ticketsDb.GetFirstOrDefaultAsync(ticket => ticket.Id == id, token);
        if (candidate == null)
        {
            return new NotFoundObjectResult($"There is no Task with {id} id");
        }

        return new OkObjectResult(_mapper.Map<List<TicketResponse>>(candidate));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] TicketRequest ticketRequest, CancellationToken token)
    {
        throw new NotImplementedException();
        // if (!ModelState.IsValid)
        // {
        //     return new BadRequestObjectResult(ticketRequest);
        // }
        //
        // var candidateProject = await _projectsDb.GetFirstOrDefaultProjectAsync(p => p.Id == ticketRequest.ProjectReferenceId, token);
        // if (candidateProject == null)
        // {
        //     return new BadRequestObjectResult($"There is no project with {ticketRequest.ProjectReferenceId} id");
        // }
        //
        // var task = new Ticket
        // {
        //     Name = ticketRequest.Name,
        //     Description = ticketRequest.Description,
        //     Project = candidateProject
        // };
        // await _ticketsDb.CreateTicketAsync(task, token);
        // return new CreatedResult("tickets", _mapper.Map<TicketResponse>(task));
    }
}
