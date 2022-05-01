using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using static PingPongMania.Constants;

namespace PingPongMania.PingService.Controllers;

[ApiController]
public class PingController : ControllerBase
{
    private readonly ILogger<PingController> _logger;

    public PingController(ILogger<PingController> logger)
    {
        _logger = logger;
    }

    [HttpGet("api/ping")]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("Ping service has been invoked.");

        try
        {
            using var client = new DaprClientBuilder().Build();
            await client.InvokeMethodAsync(HttpMethod.Get, appId: PongAppId, "api/pong");
            _logger.LogInformation("Pong service has been invoked successfully.");

            return Ok();
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed to invoke Ping service.";
            _logger.LogError(exception, errorMessage);
            return Problem(errorMessage);
        }
    }

    [Topic(PubSubName, PingTopic)]
    [HttpPost("api/ping")]
    public async Task<IActionResult> Post([FromBody] PingMessage msg)
    {
        _logger.LogInformation("Ping message Id: [{0}], Timestamp: [{1}].", msg.Id, msg.Timestamp);
        return await Get();
    }
}
