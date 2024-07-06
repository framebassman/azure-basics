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

public class ProjectControllerTests : TestBed<Fixture>
{
    private ProjectsController _projectsController;
    private TicketsController _ticketsController;
    private CosmosDbContext _db;
    private IMapper _mapper;

    public ProjectControllerTests(ITestOutputHelper testOutputHelper, Fixture fixture) : base(testOutputHelper, fixture)
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
    public async void GetAllProjects_WithNull_ReturnAllProjects()
    {
        var expected = new List<Project>
        {
            new()
            {
                Id = 1,
                Name = "Test",
                Code = "TST",
                PartitionKey = "Test",
                Tickets = new List<Ticket>()
            }
        };
        _db.Projects.AddRange(expected);
        _db.SaveChanges();

        var projectsResp = await _projectsController.GetAllAsync(null);

        Assert.IsType<OkObjectResult>(projectsResp);
        var actual = _mapper.Map<List<Project>>((IEnumerable)((OkObjectResult)projectsResp).Value);
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async void GetAllProjects_WithoutTickets_ReturnAllProjects()
    {
        var expected = new List<Project>
        {
            new()
            {
                Id = 1,
                Name = "Test",
                Code = "TST",
                PartitionKey = "Test",
                Tickets = new List<Ticket>()
            }
        };
        _db.Projects.AddRange(expected);
        _db.SaveChanges();

        var projectsResp = await _projectsController.GetAllAsync(false);

        Assert.IsType<OkObjectResult>(projectsResp);
        var actual = _mapper.Map<List<Project>>((IEnumerable)((OkObjectResult)projectsResp).Value);
        Assert.Equivalent(expected, actual);
    }


    [Fact]
    public async void GetAllProjects_WithTickets_ReturnAllProjects()
    {
        var expected = new List<Project>
        {
            new()
            {
                Id = 1,
                Name = "Test",
                Code = "TST",
                PartitionKey = "Test",
                Tickets = new List<Ticket>()
                {
                    new()
                    {
                        Id = 1,
                        Name = "TestTicket",
                        Description = "TestDesc",
                        PartitionKey = "Test",
                        ProjectReferenceId = 1
                    }
                }
            }
        };
        _db.Projects.AddRange(expected);
        _db.SaveChanges();

        var projectsResp = await _projectsController.GetAllAsync(true);

        Assert.IsType<OkObjectResult>(projectsResp);
        var actual = _mapper.Map<List<Project>>((IEnumerable)((OkObjectResult)projectsResp).Value);
        Assert.Equivalent(expected, actual);
    }
}
