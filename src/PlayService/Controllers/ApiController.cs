using Microsoft.AspNetCore.Mvc;

namespace PlayService.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiController(ILogger<ApiController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("play")]
    public IActionResult Get()
    {
        _logger.LogInformation("Play has been invoked.");
        _httpClientFactory.CreateClient("Ping").GetStringAsync("api/ping");
        return Ok("play");
    }
}
