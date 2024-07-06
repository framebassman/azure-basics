using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.DataAccess.CosmosDb;
using ProjectTasks.Documents.WebApi.Controllers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace ProjectTasks.Documents.Tests.Unit;

public class TasksControllerTests : TestBed<Fixture>
{
    private ProjectsController _projectsController;
    private TicketsController _ticketsController;
    private CosmosDbContext _db;
    private IMapper _mapper;

    public TasksControllerTests(ITestOutputHelper testOutputHelper, Fixture fixture) : base(testOutputHelper, fixture)
    {
        _projectsController = fixture.GetService<ProjectsController>(testOutputHelper);
        _ticketsController = fixture.GetService<TicketsController>(testOutputHelper);
        _db = fixture.GetService<CosmosDbContext>(testOutputHelper);
        _mapper = fixture.GetService<IMapper>(testOutputHelper);
        _db.Tickets.RemoveRange(_db.Tickets);
        _db.Projects.RemoveRange(_db.Projects);
        _db.SaveChanges();
    }

    [Fact]
    public async void GetAllTickets_WithNull_ReturnAllProjects()
    {
        var expected = new List<Ticket>
        {
            new()
            {
                Id = 1,
                Name = "Test",
                Description = "TestDesc",
                PartitionKey = "Test",
                ProjectReferenceId = 0,
            }
        };
        _db.Tickets.AddRange(expected);
        _db.SaveChanges();

        var ticketsResp = await _ticketsController.GetAllAsync(null);

        Assert.IsType<OkObjectResult>(ticketsResp);
        var actual = _mapper.Map<List<Ticket>>((IEnumerable)((OkObjectResult)ticketsResp).Value);
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async void GetAllTickets_WithoutNonExistentProjectId_ReturnNotFound()
    {
        var projectsResp = await _ticketsController.GetAllAsync("1");

        Assert.IsType<NotFoundObjectResult>(projectsResp);
        Assert.Equivalent("There is no Project with 1 id", ((NotFoundObjectResult)projectsResp).Value);
    }

    [Fact]
    public async void GetAllTickets_WithUnParseable_ReturnNotFound()
    {
        var projectsResp = await _ticketsController.GetAllAsync("a");

        Assert.IsType<NotFoundObjectResult>(projectsResp);
        Assert.Equivalent("There is no Project with a id", ((NotFoundObjectResult)projectsResp).Value);
    }

    [Fact]
    public async void GetTickets_ForSpecificProject_ReturnTickets()
    {
        var expected = new List<Ticket>
        {
            new()
            {
                Id = 1,
                Name = "TestTicket",
                Description = "TestDesc",
                PartitionKey = "Test",
                ProjectReferenceId = 1
            }
        };
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                Name = "Test",
                Code = "TST",
                PartitionKey = "Test",
                Tickets = expected
            }
        };
        _db.Projects.AddRange(projects);
        _db.SaveChanges();

        var ticketsResp = await _ticketsController.GetAllAsync(null);

        Assert.IsType<OkObjectResult>(ticketsResp);
        var actual = _mapper.Map<List<Ticket>>(
            ((OkObjectResult)ticketsResp).Value
        );
        Assert.Equivalent(expected, actual);
    }
}
