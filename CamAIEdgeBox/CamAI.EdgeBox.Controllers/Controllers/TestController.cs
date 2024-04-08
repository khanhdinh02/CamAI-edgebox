using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(
    ILogger<TestController> logger,
    IPublishEndpoint bus,
    IMemoryCache cache
) : ControllerBase
{
    [HttpGet("{name}")]
    public async Task<ActionResult<string>> TestConnection([FromRoute] string name)
    {
        logger.LogInformation($"CPU usage: {cache.Get("CpuUsage")}");
        return $"Hello {name} from edge box";
    }
}
