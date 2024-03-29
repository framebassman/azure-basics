using Microsoft.AspNetCore.Mvc;

namespace ProjectTasks.Api.Controllers;

[Route("[controller]")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("Ask Index");
        return new OkObjectResult("Hi!");
    }
}