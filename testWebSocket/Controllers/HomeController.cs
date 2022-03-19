using Microsoft.AspNetCore.Mvc;

namespace testWebSocket.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Route("/")]
    [HttpGet]
    public IActionResult Index()
    {
        return Ok();
    }
    
    [Route("/api/test")]
    [HttpGet]
    public IActionResult Test()
    {
        return Ok(new 
        {
            Name = "Cash" 
        });
    }
}