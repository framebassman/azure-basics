using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.Api.Controllers;
using ProjectTasks.Api.Models;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace ProjectTasks.Api.Tests.Unit
{
    public class UnitTest1 : TestBed<Fixture>
    {
        private ProjectsController _projectsController;
        private TasksController _tasksController;
        private ApplicationContext _db;

        public UnitTest1(ITestOutputHelper testOutputHelper, Fixture fixture) : base(testOutputHelper, fixture)
        {
            _projectsController = fixture.GetService<ProjectsController>(testOutputHelper);
            _tasksController = fixture.GetService<TasksController>(testOutputHelper);
            _db = fixture.GetService<ApplicationContext>(testOutputHelper);
            _db.Tasks.RemoveRange(_db.Tasks);
            _db.Projects.RemoveRange(_db.Projects);
            _db.SaveChanges();
        }

        [Fact]
        public void EmptyDb_GetAllProjects_ReturnNothing()
        {
            var projects = _projectsController.GetAll();
            Assert.IsType<OkObjectResult>(projects);
            Assert.Empty((IEnumerable) ((OkObjectResult) projects).Value);
        }

        [Fact]
        public void EmptyDb_GetAllTasks_ReturnNothing()
        {
            var tasks = _tasksController.GetAll();
            Assert.IsType<OkObjectResult>(tasks);
            Assert.Empty((IEnumerable) ((OkObjectResult) tasks).Value);
        }

        [Fact]
        public void CreateProject_GetAllProjects_ReturnTwoProjects()
        {
            var project = new Project { Id = 1, Code = "TST", Name = "Test" };
            var createResult = _projectsController.Create(project);
            Assert.IsType<OkResult>(createResult);

            var projects = _projectsController.GetAll();
            Assert.IsType<OkObjectResult>(projects);
            Assert.Equal(project, ((IEnumerable<Project>) ((OkObjectResult) projects).Value).First());
        }

        [Fact]
        public void CreateTwoProjects_GetAllProjects_ReturnTwoProjects()
        {
            var first = new Project { Id = 1, Code = "TST", Name = "Test" };
            var second = new Project { Id = 2, Code = "TST", Name = "Test" };
            _projectsController.Create(first);
            _projectsController.Create(second);

            var projects = _projectsController.GetAll();
            Assert.IsType<OkObjectResult>(projects);
            Assert.Equal(first, ((IEnumerable<Project>) ((OkObjectResult) projects).Value).First());
            Assert.Equal(second, ((IEnumerable<Project>) ((OkObjectResult) projects).Value).Last());
        }

        [Fact]
        public void CreateTaskWithoutProject_ReturnBadRequest()
        {
            var task = new Task { Id = 1, Name = "Test", Description = "Desc", ProjectReferenceId = 1 };
            var createResult = _tasksController.Create(task);
            Assert.IsType<BadRequestObjectResult>(createResult);
        }

        [Fact]
        public void CreateTaskWithProject_GetTask_RerturnTask()
        {
            var project = new Project { Id = 1, Code = "TST", Name = "Test" };
            _projectsController.Create(project);

            var task = new Task { Id = 1, Name = "Test", Description = "Desc", ProjectReferenceId = 1 };
            var createTaskResult = _tasksController.Create(task);
            Assert.IsType<OkResult>(createTaskResult);

            var tasks = _tasksController.GetAll();
            Assert.IsType<OkObjectResult>(tasks);
            Assert.Equal(task, ((IEnumerable<Task>) ((OkObjectResult) tasks).Value).First());
        }
    }
}
