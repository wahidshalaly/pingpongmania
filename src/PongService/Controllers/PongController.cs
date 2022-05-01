using Microsoft.AspNetCore.Mvc;

namespace PongService.Controllers;

[ApiController]
public class PongController : ControllerBase
{
    private readonly ILogger<PongController> _logger;

    public PongController(ILogger<PongController> logger)
    {
        _logger = logger;
    }

    [HttpGet("api/pong")]
    public IActionResult Get()
    {
        _logger.LogInformation("Pong service has been invoked.");
        return Ok();
    }
}
