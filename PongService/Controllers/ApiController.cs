using Microsoft.AspNetCore.Mvc;

namespace PongService.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> _logger;

    public ApiController(ILogger<ApiController> logger)
    {
        _logger = logger;
    }

    [HttpGet("pong")]
    public ActionResult<string> Get()
    {
        _logger.LogInformation("Pong has been called.");
        return Ok("pong");
    }
}
