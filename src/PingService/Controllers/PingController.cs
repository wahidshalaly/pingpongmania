using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using static PingPongMania.Constants;

namespace PingPongMania.PingService.Controllers;

[ApiController]
public class PingController : ControllerBase
{
    private readonly static DaprClient _client = new DaprClientBuilder().Build();

    private readonly ILogger<PingController> _logger;

    public PingController(ILogger<PingController> logger)
    {
        _logger = logger;
    }

    [HttpGet("api/ping")]
    public async Task<IActionResult> Reply()
    {
        _logger.LogInformation("Ping service has been invoked.");

        try
        {
            await _client.InvokeMethodAsync(HttpMethod.Get, appId: PongAppId, "api/pong");
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
    public async Task<IActionResult> Store([FromBody] PingMessage msg)
    {
        await _client.SaveStateAsync(StoreName, msg.Id.ToString(), msg);
        _logger.LogInformation("Ping message Id: [{0}], Timestamp: [{1}] has been saved.", msg.Id, msg.Timestamp);
        return await Reply();
    }
}
