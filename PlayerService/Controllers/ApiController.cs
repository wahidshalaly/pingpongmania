using Microsoft.AspNetCore.Mvc;

namespace PlayerService.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> _logger;

    public ApiController(ILogger<ApiController> logger)
    {
        _logger = logger;
    }

    [HttpGet("play")]
    public IActionResult Get()
    {
        _logger.LogInformation("Play has been called.");
        return Ok("play");
    }
}
