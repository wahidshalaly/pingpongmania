using Microsoft.AspNetCore.Mvc;

namespace PingService.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> _logger;

    public ApiController(ILogger<ApiController> logger)
    {
        _logger = logger;
    }

    [HttpGet("ping")]
    public ActionResult<string> Get()
    {
        _logger.LogInformation("Ping has been called.");
        return Ok("ping");
    }
}
