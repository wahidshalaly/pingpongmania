using Microsoft.AspNetCore.Mvc;

namespace PingService.Controllers;

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

    [HttpGet("ping")]
    public IActionResult Get()
    {
        _logger.LogInformation("Ping has been invoked.");
        _httpClientFactory.CreateClient("Pong").GetStringAsync("api/pong");
        return Ok("ping");
    }
}
