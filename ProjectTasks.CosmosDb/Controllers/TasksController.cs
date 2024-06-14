using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public IActionResult GetAllAsync()
        {
            _logger.LogInformation("Get all tasks");
            List<Models.Task> allTasks = _db.Tasks
                .ToList();
            return new OkObjectResult(allTasks);
        }
    }
}
