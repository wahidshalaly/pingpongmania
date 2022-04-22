using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace PlayService.Controllers;

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
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("Play service has been invoked.");

        try
        {
            using var client = new DaprClientBuilder().Build();
            await client.InvokeMethodAsync(HttpMethod.Get, appId: "ping-api", methodName: "api/ping");
            _logger.LogInformation("Ping service has been invoked successfully.");

            return Ok();
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed to invoke Ping service.";
            _logger.LogError(exception, errorMessage);
            return Problem(errorMessage);
        }
    }
}
