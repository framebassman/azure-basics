using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.DataAccess.AzureSQL;
using ProjectTasks.Tables.WebApi.Controllers;
using ProjectTasks.Tables.WebApi.Models;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace ProjectTasks.Api.Tests.Unit;

public class ProjectControllerTests : TestBed<Fixture>
{
    private ProjectsController _projectsController;
    private TicketsController _ticketsController;
    private AzureSqlDbContext _db;
    private IMapper _mapper;

    public ProjectControllerTests(ITestOutputHelper testOutputHelper, Fixture fixture) : base(testOutputHelper, fixture)
    {
        _projectsController = fixture.GetService<ProjectsController>(testOutputHelper);
        _ticketsController = fixture.GetService<TicketsController>(testOutputHelper);
        _db = fixture.GetService<AzureSqlDbContext>(testOutputHelper);
        _mapper = fixture.GetService<IMapper>(testOutputHelper);
        _db.Tasks.RemoveRange(_db.Tasks);
        _db.Projects.RemoveRange(_db.Projects);
        _db.SaveChanges();
    }

    [Fact]
    public async void EmptyDb_GetAllProjects_ReturnNothing()
    {
        var projects = await _projectsController.GetAllAsync();
        Assert.IsType<OkObjectResult>(projects);
        Assert.Empty((IEnumerable) ((OkObjectResult) projects).Value);
    }

    [Fact]
    public async void CreateProject_GetAllProjects_OneProject()
    {
        var projectRequest = new ProjectRequest { Code = "TST", Name = "Test" };
        var createResult = await _projectsController.CreateAsync(projectRequest);
        Assert.IsType<CreatedResult>(createResult);

        var projects = await _projectsController.GetAllAsync();
        Assert.IsType<OkObjectResult>(projects);
        var projectResponse = ((IEnumerable<ProjectResponse>)((OkObjectResult) projects).Value).First();
        Assert.Equal(projectRequest.Code, projectResponse.Code);
        Assert.Equal(projectRequest.Name, projectResponse.Name);
    }

    [Fact]
    public async void CreateTwoProjects_GetAllProjects_ReturnTwoProjects()
    {
        var first = new ProjectRequest { Code = "TST", Name = "Test" };
        var second = new ProjectRequest { Code = "TST", Name = "Test" };
        await _projectsController.CreateAsync(first);
        await _projectsController.CreateAsync(second);

        var projects = await _projectsController.GetAllAsync();
        Assert.IsType<OkObjectResult>(projects);
        var firstProjectResponse = ((IEnumerable<ProjectResponse>)((OkObjectResult) projects).Value).First();
        Assert.Equal(firstProjectResponse.Code, first.Code);
        Assert.Equal(firstProjectResponse.Name, first.Name);
        var secondProjectResponse = ((IEnumerable<ProjectResponse>)((OkObjectResult) projects).Value).Last();
        Assert.Equal(secondProjectResponse.Code, second.Code);
        Assert.Equal(secondProjectResponse.Name, second.Name);
    }
}
