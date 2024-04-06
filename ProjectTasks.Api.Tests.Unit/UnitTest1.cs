using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.Api.Controllers;
using ProjectTasks.Api.Models;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Task = ProjectTasks.Api.Models.Task;

namespace ProjectTasks.Api.Tests.Unit
{
    public class UnitTest1 : TestBed<Fixture>
    {
        private ProjectsController _projectsController;
        private TasksController _tasksController;
        private ApplicationContext _db;
        private IMapper _mapper;

        public UnitTest1(ITestOutputHelper testOutputHelper, Fixture fixture) : base(testOutputHelper, fixture)
        {
            _projectsController = fixture.GetService<ProjectsController>(testOutputHelper);
            _tasksController = fixture.GetService<TasksController>(testOutputHelper);
            _db = fixture.GetService<ApplicationContext>(testOutputHelper);
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
        public async void EmptyDb_GetAllTasks_ReturnNothing()
        {
            var tasks = await _tasksController.GetAllAsync();
            Assert.IsType<OkObjectResult>(tasks);
            Assert.Empty((IEnumerable) ((OkObjectResult) tasks).Value);
        }

        [Fact]
        public async void CreateProject_GetAllProjects_ReturnTwoProjects()
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

        [Fact]
        public async void CreateTaskWithoutProject_ReturnBadRequest()
        {
            var taskRequest = new TaskRequest { Name = "Test", Description = "Desc", ProjectReferenceId = 1 };
            var createResult = await _tasksController.CreateAsync(taskRequest);
            Assert.IsType<BadRequestObjectResult>(createResult);
        }

        [Fact]
        public async void CreateTaskWithProject_GetTask_RerturnTask()
        {
            var projectRequest = new ProjectRequest { Code = "TST", Name = "Test" };
            var projectResponse = await _projectsController.CreateAsync(projectRequest);
            Assert.IsType<CreatedResult>(projectResponse);

            var taskRequest = new TaskRequest
            {
                Name = "Test",
                Description = "Desc",
                ProjectReferenceId = ((ProjectResponse)((CreatedResult)projectResponse).Value).Id
            };
            var createTaskResult = await _tasksController.CreateAsync(taskRequest);
            Assert.IsType<CreatedResult>(createTaskResult);

            var tasks = await _tasksController.GetAllAsync();
            Assert.IsType<OkObjectResult>(tasks);
            var taskResponses = (IEnumerable<TaskResponse>)((OkObjectResult)tasks).Value;
            Assert.Equal(taskResponses.First().Name, ((IEnumerable<TaskResponse>) ((OkObjectResult) tasks).Value).First().Name);
            Assert.Equal(taskResponses.First().Description, ((IEnumerable<TaskResponse>) ((OkObjectResult) tasks).Value).First().Description);
            Assert.Equal(taskResponses.First().ProjectReferenceId, ((IEnumerable<TaskResponse>) ((OkObjectResult) tasks).Value).First().ProjectReferenceId);
        }
    }
}
