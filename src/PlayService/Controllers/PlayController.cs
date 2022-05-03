using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using static PingPongMania.Constants;

namespace PingPongMania.PlayService.Controllers;

[ApiController]
public class PlayController : ControllerBase
{
    private readonly static DaprClient _client = new DaprClientBuilder().Build();

    private readonly ILogger<PlayController> _logger;

    public PlayController(ILogger<PlayController> logger)
    {
        _logger = logger;
    }

    [HttpGet("api/play/{method=http}")]
    public async Task<IActionResult> Get(Transport method = Transport.Http)
    {
        _logger.LogInformation("Play service has been invoked via {method}.", method);

        try
        {
            if (method == Transport.PubSub)
                await SendViaPubSub();
            else
                await SendViaHttp();

            _logger.LogInformation("Ping service has been called via {method}.", method);
            return Ok();
        }
        catch (Exception exception)
        {
            return ReportProblem(method, exception);
        }
    }

    private static async Task SendViaHttp()
    {
        await _client.InvokeMethodAsync(HttpMethod.Get, appId: PingAppId, "api/ping");
    }

    private static async Task SendViaPubSub()
    {
        await _client.PublishEventAsync(PubSubName, PingTopic, CreateMessage());
    }

    private static PingMessage CreateMessage() => new(Guid.NewGuid(), DateTimeOffset.UtcNow);

    private ObjectResult ReportProblem(Transport method, Exception exception)
    {
        var errorMessage = $"Failed to call Ping service via {method}.";
        _logger.LogError(exception, errorMessage);

        return Problem(errorMessage);
    }
}

public enum Transport {
    Http = 0,
    PubSub = 1
}
